using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Myna.Web.Core.Configuration {
    public static class SwaggerConfigurationExtensions {

        public static void AddCustomSwagger(this IServiceCollection services) {
            services.AddSwaggerExamples();
            services.AddSwaggerGen(_ => {
                _.EnableAnnotations();
                _.SwaggerDoc("Myna-Server API v1.0", new Info { Version = "v1.0", Title = "Myna-Server" });
                _.OperationFilter<RemoveVersionParameters>();
                _.DocumentFilter<SetVersionInPaths>();
                _.DocInclusionPredicate((docName, apiDesc) => {
                    if (!apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)) return false;

                    var versions = methodInfo.DeclaringType
                        .GetCustomAttributes<ApiVersionAttribute>(true)
                        .SelectMany(attr => attr.Versions);

                    return versions.Any(v => $"v{v.ToString()}" == docName);
                });
            });
        }

        public static void UseCustomSwagger(this IApplicationBuilder app) {

        }
    }

    public class RemoveVersionParameters : IOperationFilter {
        public void Apply(Operation operation, OperationFilterContext context) {
            // Remove version parameter from all Operations
            var versionParameter = operation.Parameters.SingleOrDefault(p => p.Name == "version");
            if (versionParameter != null)
                operation.Parameters.Remove(versionParameter);
        }
    }

    public class SetVersionInPaths : IDocumentFilter {
        public void Apply(SwaggerDocument swaggerDoc, DocumentFilterContext context) {
            var updatedPaths = new Dictionary<string, PathItem>();

            foreach (var entry in swaggerDoc.Paths) {
                updatedPaths.Add(
                    entry.Key.Replace("v{version}", swaggerDoc.Info.Version),
                    entry.Value);
            }

            swaggerDoc.Paths = updatedPaths;
        }
    }

}
