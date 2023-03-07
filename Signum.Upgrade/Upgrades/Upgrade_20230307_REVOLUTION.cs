using LibGit2Sharp;
using Signum.Utilities;
using System;
using System.Collections.Generic;
using System.IO;

namespace Signum.Upgrade.Upgrades;

class Upgrade_20230307_REVOLUTION : CodeUpgradeBase
{
    public override string Description => "Updates Nugets";

    public override void Execute(UpgradeContext uctx)
    {
        ExtractExtensions(uctx, "Alerts");
        ExtractExtensions(uctx, "Authorization");
        CreateEmptyExtensionsProject(uctx, "ResetPassword");
        CreateEmptyExtensionsProject(uctx, "ActiveDirectory");
        ExtractExtensions(uctx, "Cache", projectName: "Signum.Caching");
        ExtractExtensions(uctx, "Calendar");
        ExtractExtensions(uctx, "Chart");
        ExtractExtensions(uctx, "ConcurrentUser");
        ExtractExtensions(uctx, "Dashboard");
        ExtractExtensions(uctx, "DiffLog");
        CreateEmptyExtensionsProject(uctx, "TimeMachine");
        ExtractExtensions(uctx, "Disconnected");
        ExtractExtensions(uctx, "Discovery");
        ExtractExtensions(uctx, "Dynamic");
        ExtractExtensions(uctx, "Excel");
        ExtractExtensions(uctx, "Files");
        ExtractExtensions(uctx, "Help");
        ExtractExtensions(uctx, "Isolation");
        ExtractExtensions(uctx, "MachineLearning");
        ExtractExtensions(uctx, "Mailing");
        CreateEmptyExtensionsProject(uctx, "MailPackage");
        ExtractExtensions(uctx, "Map");
        ExtractExtensions(uctx, "Migrations");
        ExtractExtensions(uctx, "Notes");
        ExtractExtensions(uctx, "Omnibox");
        ExtractExtensions(uctx, "Printing");
        ExtractExtensions(uctx, "Processes");
        ExtractExtensions(uctx, "Profiler");
        ExtractExtensions(uctx, "Rest");
        ExtractExtensions(uctx, "Scheduler");
        ExtractExtensions(uctx, "SMS");
        ExtractExtensions(uctx, "Templating");
        ExtractExtensions(uctx, "Toolbar");
        ExtractExtensions(uctx, "Translation");
        ExtractExtensions(uctx, "Tree");
        ExtractExtensions(uctx, "UserAssets");
        ExtractExtensions(uctx, "UserQueries");
        ExtractExtensions(uctx, "ViewLog");
        ExtractExtensions(uctx, "WhatsNew");
        ExtractExtensions(uctx, "Word");
        ExtractExtensions(uctx, "Workflow");
    }

    static void ExtractExtensions(UpgradeContext uctx, string folderName, string? projectName = null) 
    {
        if (projectName == null)
            projectName = "Signum." + folderName;

        CreateCsharpProject(uctx, "Framework/Extensions/", projectName);
        MoveAllFiles(uctx, "Framework/Extensions/" + projectName, "Framework/Signum.Entities.Extensions/" + folderName, "*.*");

        CopyAndRenameTranslations(uctx, "Framework/Extensions/" + projectName + "/Translations", "Framework/Signum.Entities.Extensions/Translations", projectName);
        MoveAllFiles(uctx, "Framework/Extensions/" + projectName, "Framework/Signum.Engine.Extensions/" + folderName, "*.*");
        MoveAllFiles(uctx, "Framework/Extensions/" + projectName, "Framework/Signum.React.Extensions/" + folderName, "*.cs");

        string reactProjectName = projectName + ".React";

        CreateReactProject(uctx, "Framework/Extensions/", reactProjectName);
        MoveAllFiles(uctx, "Framework/Extensions/" + reactProjectName, "Framework/Signum.React.Extensions/" + folderName, "*.tsx");
        MoveAllFiles(uctx, "Framework/Extensions/" + reactProjectName, "Framework/Signum.React.Extensions/" + folderName, "*.ts");

        CommitFramework(uctx, "Extract " + projectName);
    }

    static void CreateEmptyExtensionsProject(UpgradeContext uctx, string folderName)
    {
        var projectName = "Signum." + folderName;
        string reactProjectName = "Signum." + folderName + ".React";

        CreateCsharpProject(uctx, "Framework/Extensions/", projectName);
        CreateReactProject(uctx, "Framework/Extensions/", reactProjectName);

        CommitFramework(uctx, "Create empty " + projectName);
    }

    private static void CommitFramework(UpgradeContext uctx, string message)
    {
        using (Repository rep = new Repository(uctx.AbsolutePath("Framework")))
        {
            if (rep.RetrieveStatus().IsDirty)
            {
                Commands.Stage(rep, "*");
                var sign = rep.Config.BuildSignature(DateTimeOffset.Now);
                rep.Commit(message, sign, sign);
                SafeConsole.WriteLineColor(ConsoleColor.White, "A commit with text message '{0}' has been created".FormatWith(message));
            }
            else
            {
                Console.WriteLine("Nothing to commit");
            }
        }
    }

    private static void MoveAllFiles(UpgradeContext uctx, string destination, string source, string searchPattern)
    {
        destination = uctx.AbsolutePath(destination);
        source = uctx.AbsolutePath(source);

        if (!Directory.Exists(source))
            return;

        foreach (var item in uctx.GetCodeFiles(source, new[] { searchPattern }, UpgradeContext.DefaultIgnoreDirectories))
        {
            var filePath = uctx.AbsolutePath(item.FilePath);
            var destFilePath = Path.Combine(destination, Path.GetRelativePath(source, filePath));

            var dir = Path.GetDirectoryName(destFilePath)!;
            Directory.CreateDirectory(dir);

            File.Move(filePath, destFilePath);
        } 
    }

    private static void CopyAndRenameTranslations(UpgradeContext uctx, string destination, string source, string projectName)
    {
        source = uctx.AbsolutePath(source);
        destination = uctx.AbsolutePath(destination);

        if (!Directory.Exists(source))
            return;

        foreach (var item in uctx.GetCodeFiles(source, new[] { "*.xml" }, UpgradeContext.DefaultIgnoreDirectories))
        {
            var filePath = uctx.AbsolutePath(item.FilePath);
            var culture = filePath.BeforeLast(".").AfterLast(".");

            var destFilePath = Path.Combine(destination, projectName + "." + culture + ".xml");

            var dir = Path.GetDirectoryName(destFilePath)!;
            Directory.CreateDirectory(dir);

            File.Copy(filePath, destFilePath);
        }
    }

    static void CreateCsharpProject(UpgradeContext uctx, string directory, string projectName)
    {
        var csharpDirectory = uctx.AbsolutePath(Path.Combine(directory, projectName));

        Directory.CreateDirectory(csharpDirectory);


        uctx.CreateCodeFile(
            Path.Combine(csharpDirectory, projectName + ".csproj"),
            $"""
            <Project Sdk="Microsoft.NET.Sdk">

            	<PropertyGroup>
            		<TargetFramework>net7.0</TargetFramework>
            		<Nullable>enable</Nullable>
            		<WarningsAsErrors>nullable</WarningsAsErrors>
            		<OutputType>Library</OutputType>
            		<NoWarn>8618</NoWarn>
            	</PropertyGroup>
  
            	<ItemGroup>
            		<FrameworkReference Include="Microsoft.AspNetCore.App" />
            		<PackageReference Include="Signum.Analyzer" Version="3.2.0" />
            		<PackageReference Include="Signum.MSBuildTask" Version="7.5.0-beta" />
            		<PackageReference Include="Signum.TSGenerator" Version="7.5.0-beta2" />
            	</ItemGroup>

            	<ItemGroup>
            		<ProjectReference Include="{Path.GetRelativePath(csharpDirectory, uctx.AbsolutePath("Framework\\Signum.Utilities\\Signum.Utilities.csproj"))}" />
            		<ProjectReference Include="{Path.GetRelativePath(csharpDirectory, uctx.AbsolutePath("Framework\\Signum\\Signum.csproj"))}" />
            	</ItemGroup>

                <ItemGroup>
            	    <None Update="Translations\*.xml">
            		    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
            	    </None>
                </ItemGroup>
            </Project>
            """);


        var properties = Directory.CreateDirectory(Path.Combine(csharpDirectory, "Properties"));

        uctx.CreateCodeFile(
            Path.Combine(properties.FullName, "Attributes.cs"),
            $"""
            [assembly: DefaultAssemblyCulture("en")]
            """);

        uctx.CreateCodeFile(
            Path.Combine(properties.FullName, "GlobalUsings.cs"),
            $"""
            global using System;
            global using System.Collections.Generic;
            global using System.Linq;
            global using System.Linq.Expressions;
            global using System.Text;
            global using System.Reflection;
            global using Signum.Utilities;
            global using Signum.Utilities.ExpressionTrees;
            global using Signum.Entities;
            global using Signum.Entities.Reflection;
            global using Signum.Entities.Validation;
            global using Signum.DynamicQuery;
            global using Signum.Operations;
            global using Signum.Engine;
            global using Signum.Basics;
            global using Signum.Security;
            global using System.Threading.Tasks;
            global using System.Threading;
            global using Signum.Engine.Maps;
            
            """);
    }

    static void CreateReactProject(UpgradeContext uctx, string directory, string projectName)
    {
        var reactDirectory = uctx.AbsolutePath(Path.Combine(directory, projectName));
        Directory.CreateDirectory(reactDirectory);
        uctx.CreateCodeFile(
            Path.Combine(reactDirectory, projectName + ".esproj"),
            $"""
            <Project Sdk="Microsoft.VisualStudio.JavaScript.Sdk/0.5.74-alpha">
              <PropertyGroup>
                <BuildCommand>yarn run tsc</BuildCommand>
              </PropertyGroup>
              <ItemGroup>
                <TypeScriptConfiguration Include="tsconfig.base.json" />
              </ItemGroup>
            </Project>
            """);

        uctx.CreateCodeFile(
            Path.Combine(reactDirectory, "nuget.config"),
            $"""
            <?xml version="1.0" encoding="utf-8"?>
            <configuration>
              <packageSources>
                <clear />
                <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
              </packageSources>
              <disabledPackageSources>
                <clear />
              </disabledPackageSources>
            </configuration>
            """);

        uctx.CreateCodeFile(
    Path.Combine(reactDirectory, "tsconfig.json"),
            $$"""
            {
              "extends": "./tsconfig.base"
            }
            """);

        uctx.CreateCodeFile(
            Path.Combine(reactDirectory, "package.json"),
            $$"""
            {
              "name": "{{projectName.SpacePascal().ToLower().Split(" ").ToString("-")}}",
              "version": "1.0.0",
              "description": "Resable modules to use on top of Signum Framework",
              "repository": "",
              "keywords": [],
              "author": "Signum Software",
              "license": "MIT",
              "resolutions": {
              },
              "dependencies": {
              }
            }
            """);
    }
}
