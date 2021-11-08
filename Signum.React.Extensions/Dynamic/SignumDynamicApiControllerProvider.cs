using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Signum.Engine.Dynamic;
using Signum.Entities.Dynamic;
using System.IO;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Signum.React.Extensions.Dynamic
{
    public class SignumDynamicApiControllerProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            if (DynamicCode.CodeGenControllerAssemblyPath.HasText())
            {
                var assembly = Assembly.LoadFrom(DynamicCode.CodeGenControllerAssemblyPath);
                var candidates = assembly.GetExportedTypes();

                foreach (var candidate in candidates)
                {
                    feature.Controllers.Add(candidate.GetTypeInfo());
                }
            }
        }
    }
}
