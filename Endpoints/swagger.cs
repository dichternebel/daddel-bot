using System.Net.Http;
using System.Threading.Tasks;
using AzureFunctions.Extensions.Swashbuckle;
using AzureFunctions.Extensions.Swashbuckle.Attribute;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;

public static class swagger
{
    [SwaggerIgnore]
    [FunctionName("swaggerJs")]
    public static Task<HttpResponseMessage> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger/json")] HttpRequestMessage req,
        [SwashBuckleClient]ISwashBuckleClient swashBuckleClient)
    {
        return Task.FromResult(swashBuckleClient.CreateSwaggerDocumentResponse(req));
    }

    [SwaggerIgnore]
    [FunctionName("swaggerUi")]
    public static Task<HttpResponseMessage> Run2(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "swagger/ui")] HttpRequestMessage req,
        [SwashBuckleClient]ISwashBuckleClient swashBuckleClient)
    {
        return Task.FromResult(swashBuckleClient.CreateSwaggerUIResponse(req, "swagger/json"));
    }
}