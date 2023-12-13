using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FormAzureFunction
{
    public class GetPDF
    {
        private readonly ILogger<GetPDF> _logger;

        public GetPDF(ILogger<GetPDF> log)
        {
            _logger = log;
        }

        [FunctionName("GetPDF")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "submissionId" })]
        [OpenApiParameter(name: "submissionId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **SubmissionId** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            string submissionId = req.Query["submissionId"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            submissionId = submissionId ?? data?.submissionId;

            string responseMessage = string.IsNullOrEmpty(submissionId)
                ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
                : $"Hello, {submissionId}. Action OK.";




            return new OkObjectResult(responseMessage);
        }
    }
}
