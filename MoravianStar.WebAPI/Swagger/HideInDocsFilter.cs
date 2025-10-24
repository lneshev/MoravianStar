using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.OpenApi.Models;
using MoravianStar.WebAPI.Attributes;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Linq;
using System.Reflection;

namespace MoravianStar.WebAPI.Swagger
{
    /// <summary>
    /// This <see cref="IDocumentFilter"/> hides from the documentation every endpoint that is marked with <see cref="NonInvokableAttribute"/>.
    /// </summary>
    public class HideInDocsFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            foreach (var apiDescription in context.ApiDescriptions)
            {
                if (apiDescription.ActionDescriptor is ControllerActionDescriptor)
                {
                    var actionDescriptor = (ControllerActionDescriptor)apiDescription.ActionDescriptor;

                    if (actionDescriptor.MethodInfo.GetCustomAttributes<NonInvokableAttribute>().Any())
                    {
                        var key = "/" + apiDescription.RelativePath.TrimEnd('/');
                        var operation = (OperationType)Enum.Parse(typeof(OperationType), apiDescription.HttpMethod, true);

                        // Drop the operation
                        swaggerDoc.Paths[key].Operations.Remove(operation);

                        // Drop the entire route if there are no operations left
                        if (!swaggerDoc.Paths[key].Operations.Any())
                        {
                            swaggerDoc.Paths.Remove(key);
                        }
                    }
                }
            }
        }
    }
}