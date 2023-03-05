using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Concurrent;
using System.Diagnostics;
using Mono.Cecil;
using System.CodeDom.Compiler;
using Mono.Cecil.Pdb;
using System.Xml.Linq;
using System.Reflection.PortableExecutable;

namespace Signum.TSGenerator;

public static class Program
{
    public static int Main(string[] args)
    {
        var log = Console.Out;
        Stopwatch sw = Stopwatch.StartNew();
        string currentCsproj = "";
        try
        {
            string intermediateAssembly = args[0];
            string referencesFile = args[1];


            currentCsproj = Path.Combine(Directory.GetCurrentDirectory(), Path.GetFileNameWithoutExtension(intermediateAssembly) + ".csproj");
            if (!File.Exists(currentCsproj))
                throw new InvalidOperationException($"Project file not found in ({currentCsproj})");

            var currentReactDirectory = FindReactDirectory(currentCsproj);
            if (!Directory.Exists(currentReactDirectory))
                throw new InvalidOperationException($"No React Directory found ({currentReactDirectory})");

            log.WriteLine("Starting SignumTSGenerator");

            var currentT4Files = GetAllT4SFiles(currentReactDirectory);
            var signumUpToDatePath = Path.Combine(Path.GetDirectoryName(intermediateAssembly), "SignumUpToDate.txt");

            if (File.Exists(signumUpToDatePath))
            {
                var upToDateContent = GetUpToDateContent(intermediateAssembly, currentT4Files);
                if (File.ReadAllText(signumUpToDatePath) == upToDateContent) {

                    log.WriteLine($"SignumTSGenerator already processed ({sw.ElapsedMilliseconds.ToString()}ms)");
                    return 0;
                }
            }

            string[] references = File.ReadAllLines(referencesFile);
            var assemblyLocations = references.ToDictionary(a => Path.GetFileNameWithoutExtension(a));

            if (!assemblyLocations.ContainsKey("Signum") && Path.GetFileName(currentCsproj) == "Signum.csproj")
                assemblyLocations.Add("Signum", intermediateAssembly);

            var projXml = XDocument.Load(currentCsproj);
            var candidates = projXml.Document.Descendants("ProjectReference").Select(a => a.Attribute("Include").Value).Prepend(currentCsproj).ToList();
            var assemblyReferences = (from csproj in candidates
                                      where ReferencesOrIsSignum(csproj)
                                      let reactDirectory = FindReactDirectory(csproj)
                                      let assemblyName = Path.GetFileNameWithoutExtension(csproj)
                                      select new AssemblyReference
                                      {
                                          AssemblyName = assemblyName,
                                          AssemblyFullPath = assemblyLocations[assemblyName],
                                          ReactDirectory = reactDirectory,
                                          AllTypescriptFiles = GetAllT4SFiles(reactDirectory),
                                      }).ToDictionary(a => a.AssemblyName);


            var entityResolver = new PreloadingAssemblyResolver(assemblyLocations);
            var currentModule = ModuleDefinition.ReadModule(intermediateAssembly, new ReaderParameters
            {
                AssemblyResolver = entityResolver
            });

            var options = new AssemblyOptions
            {
                CurrentAssembly = intermediateAssembly,
                AssemblyReferences = assemblyReferences,
                AllReferences = references.ToDictionary(a => Path.GetFileNameWithoutExtension(a)),
                ModuleDefinition = currentModule,
                Resolver = entityResolver,
            };


            var tsTypes = EntityDeclarationGenerator.GetAllTSTypes(options);

            var shouldT4s = tsTypes.GroupBy(a => a.Namespace).ToDictionary(gr => gr.Key, gr => gr.ToList());
            var currentT4s = currentT4Files.ToDictionary(a => Path.GetFileNameWithoutExtension(a));

            var extra = currentT4s.Where(kvp => !shouldT4s.ContainsKey(kvp.Key)).ToList();

            if (extra.Any())
                throw new InvalidOperationException($"The following t4s {(extra.Count == 1? "file is" : "files are")} not needed:\r\n" + string.Join("\r\n",  extra.Select(a=>a.Value)));


            var missing = shouldT4s.Where(kvp => !currentT4s.ContainsKey(kvp.Key)).ToList();
            foreach (var m in missing)
            {
                var newT4S = Path.Combine(currentReactDirectory, m.Key + ".t4s");
                currentT4Files.Add(newT4S);
                currentT4s.Add(m.Key, newT4S);
                log.WriteLine($"Automatically creating {newT4S}");
                File.WriteAllBytes(newT4S, new byte[0]);
            }

            foreach (var kvp in shouldT4s)
            {
                var t4sFile = currentT4s[kvp.Key];
                string result = EntityDeclarationGenerator.WriteNamespaceFile(options, t4sFile, kvp.Key, kvp.Value);

                var targetFile = Path.ChangeExtension(t4sFile, ".ts");
                if (File.Exists(targetFile) && File.ReadAllText(targetFile) == result)
                {
                    log.WriteLine($"Skipping {targetFile} (Up to date)");
                }
                else
                {
                    log.WriteLine($"Writing {targetFile}");
                    File.WriteAllText(targetFile, result);
                }
            }

            {
                var upToDateContent = GetUpToDateContent(intermediateAssembly, currentT4Files);
                File.WriteAllText(signumUpToDatePath, upToDateContent);
            }
            log.WriteLine($"SignumTSGenerator finished ({sw.ElapsedMilliseconds}ms)");
            Console.WriteLine();
            return 0;

        }
        catch (Exception ex)
        {
            log.WriteLine($"SignumTSGenerator finished with errors ({sw.ElapsedMilliseconds}ms)");
            log.WriteLine($"{currentCsproj ?? "" }:error STSG0001:{ex.Message}");
            log.WriteLine(ex.Message);
            return -1;
        }
    }

    private static string GetUpToDateContent(string intermediateAssembly, List<string> t4Files)
    {
        return string.Join("\r\n", new[] { intermediateAssembly }.Concat(t4Files.OrderBy(a => a))
                     .Select(f => File.GetLastWriteTimeUtc(f).ToString("o") + " " + Path.GetFileName(f))
                    .ToList()
                );
    }

    private static bool ReferencesOrIsSignum(string csprojFilePath)
    {
        if (Path.GetFileName(csprojFilePath) == "Signum.csproj")
            return true;

        var projectReferences = XDocument.Load(csprojFilePath).Document.Descendants("ProjectReference").ToList();
        return projectReferences.Any(p => Path.GetFileName(p.Value) == "Signum.csproj");
    }

    static string FindReactDirectory(string csproFilePath)
    {
        var projectDirectory = Path.GetDirectoryName(csproFilePath);

        var projectName = Path.GetFileName(projectDirectory);

        return Path.Combine(Path.GetDirectoryName(projectDirectory), projectName + ".React");
    }

    public static List<string> GetAllT4SFiles(string reactDirectory)
    {
        var result = new List<string>();

        void Fill(string dir)
        {
            result.AddRange(Directory.EnumerateFiles(dir, "*.t4s"));

            foreach (var subDir in Directory.EnumerateDirectories(dir))
            {
                var subDirName = Path.GetFileName(subDir);
                if (subDirName == "obj" || subDirName == "bin" || subDirName == "node_modules" || subDirName == "ts_out")
                    continue;

                Fill(subDir);
            }
        }

        Fill(reactDirectory);

        return result;
    }
}
