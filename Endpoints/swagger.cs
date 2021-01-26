using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using Newtonsoft.Json;
using System.Web;
using System.Net.Http;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Net;
using System.Linq;
using System.Text;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration;
using Microsoft.OpenApi.Extensions;
using Microsoft.OpenApi.CSharpAnnotations.DocumentGeneration.Models;

namespace Rcon.Function
{
    public static class swagger
    {
        /// <summary>
        /// gives back the OpenAPI json document
        /// </summary>
        /// <group>apimanagement</group>
        /// <verb>GET/verb>
        /// <url>https://rcon.azurewebsites.net/api/swagger</url>
        /// <remarks>generates swagger API</remarks>
        /// <response code="200">successful operation and response payload</response>
        [FunctionName("swagger")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log)
        {
            var input = new OpenApiGeneratorConfig(
                annotationXmlDocuments: new List<XDocument>()
                {
                    XDocument.Load(@"RconFunctionsOpenAPI.xml"),
                },
                assemblyPaths: new List<string>()
                {
                    @"bin\rcon-function.dll"
                },
                openApiDocumentVersion: "V1",
                filterSetVersion: FilterSetVersion.V1
            );
            input.OpenApiInfoDescription = "This is the OpenAPI description for the rcon function.";

            var generator = new OpenApiGenerator();
            var openApiDocuments = generator.GenerateDocuments(
                openApiGeneratorConfig: input,
                generationDiagnostic: out GenerationDiagnostic result
            );

            return new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = new StringContent(openApiDocuments.First().Value.SerializeAsJson(OpenApiSpecVersion.OpenApi3_0), Encoding.UTF8, "application/json")
            };
        }
    }
}