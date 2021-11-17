using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Settings;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Rcon.Function;
using Swashbuckle.AspNetCore.SwaggerGen;

[assembly: WebJobsStartup(typeof(SwashBuckleStartup))]
namespace Rcon.Function
{
    internal class SwashBuckleStartup : IWebJobsStartup
    {
        //https://stackoverflow.com/questions/52883466/how-to-add-method-description-in-swagger-ui-in-webapi-application
        public void Configure(IWebJobsBuilder builder)
        {
            //Register the extension
            //builder.AddSwashBuckle(Assembly.GetExecutingAssembly());
            builder.AddSwashBuckle(Assembly.GetExecutingAssembly(), opts =>
            {
                opts.SpecVersion = OpenApiSpecVersion.OpenApi3_0;
                opts.AddCodeParameter = false;
                opts.PrependOperationWithRoutePrefix = true;
                opts.Documents = new []
                {
                    new SwaggerDocument
                    {
                        Name = "v1",
                        Title = "RCON Function (CSGO)",
                        Description = "Swagger document for RCON function used by 'daddelbot'.<br />Invite link:<a href=\"https://discord.com/oauth2/authorize?client_id=797866820996169779&permissions=93248&scope=bot\">https://discord.com/oauth2/authorize?client_id=797866820996169779&permissions=93248&scope=bot</a>",
                        Version = "v3"
                    }
                };
                opts.Title = "RCON Function";
                //opts.OverridenPathToSwaggerJson = new Uri("http://localhost:7071/api/Swagger/json");
                opts.ConfigureSwaggerGen = (x =>
                {
                    x.CustomOperationIds(apiDesc =>
                    {
                        return apiDesc.TryGetMethodInfo(out MethodInfo methodInfo)
                            ? methodInfo.Name
                            : new Guid().ToString();
                    });

                    x.OperationFilter<AddRequiredHeaderParameter>();
                });
            });
        }
    }

    internal class AddRequiredHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
                {
                    Name = " x-functions-key",
                    In = ParameterLocation.Header,
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "String",
                        Default = new OpenApiString("")
                    }
                });

           operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "accessToken",
                    In = ParameterLocation.Header,
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "String",
                        Default = new OpenApiString("")
                    }
                });
        }
    }
}