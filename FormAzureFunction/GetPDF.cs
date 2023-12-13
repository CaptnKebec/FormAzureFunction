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
using System.Net.Http;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Microsoft.CodeAnalysis;

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

            //  string submissionId = req.Query["submissionId"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            //  submissionId = submissionId ?? data?.submissionId;

            //string responseMessage = string.IsNullOrEmpty(submissionId)
            //    ? "This HTTP triggered function executed successfully. Pass a name in the query string or in the request body for a personalized response."
            //    : $"Hello, {submissionId}. Action OK.";
            string baseUrl = "https://devformioapi.hema-quebec.qc.ca/api-dev/";

            loginRequest objToSend = new loginRequest();
            objToSend.data = new loginObject();
            objToSend.data.email = "bob@aa.com";
            objToSend.data.password = "Hemaqc01";



            string json = JsonConvert.SerializeObject(objToSend);


            HttpRequestMessage request1 = new HttpRequestMessage()
            {
                Method = HttpMethod.Post,
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"),
                RequestUri = new Uri(baseUrl + "user/login")
            };

            HttpClient httpClient = new HttpClient();

            HttpResponseMessage response = await httpClient.SendAsync(request1);
            //var obj = await response.Content.ReadAsStringAsync();
            var token = response.Headers.FirstOrDefault(i => i.Key == "x-jwt-token").Value.FirstOrDefault();

            HttpRequestMessage request2 = new HttpRequestMessage()
            {
                Headers = { { "x-jwt-token", token } },
                Method = HttpMethod.Get,
                RequestUri = new Uri(baseUrl + "helloworld/submission")
            };

            HttpResponseMessage response2 = await httpClient.SendAsync(request2);
            var obj = await response2.Content.ReadAsStringAsync();

            dynamic submissionObj = JsonConvert.DeserializeObject<dynamic>(obj);


            dynamic firstSub = submissionObj[0];

            string submissionId = firstSub._id;
            string submissionFormId = firstSub.form;
            string submissionProjectId = firstSub.project;

            loginRequest objToSend2 = new loginRequest();
            objToSend2.data = new loginObject();
            objToSend2.data.email = "michel.bernier@hema-quebec.qc.ca";
            objToSend2.data.password = "Hemaqc01";
            string json2 = JsonConvert.SerializeObject(objToSend2);

            HttpRequestMessage request3 = new HttpRequestMessage()
            {
                
                Method = HttpMethod.Post,
                RequestUri = new Uri(baseUrl + "admin/login"),
                Content = new StringContent(json2, System.Text.Encoding.UTF8, "application/json"),
            };

            HttpResponseMessage response3 = await httpClient.SendAsync(request3);
            var adminToken = response3.Headers.FirstOrDefault(i => i.Key == "x-jwt-token").Value.FirstOrDefault();

            //string allowURL = "GET:/project/" +
            //        submissionProjectId +
            //        "/form/" +
            //        submissionFormId +
            //        "/submission/" +
            //        submissionId +
            //        "/download";

            //HttpRequestMessage request4 = new HttpRequestMessage()
            //{
            //    Headers = {
            //        { "x-jwt-token", adminToken  } ,
            //        { "x-expire", "3600"  },
            //        { "x-allow", allowURL }
            //    },
            //    Method = HttpMethod.Get,
            //    RequestUri = new Uri("https://devformioapi.hema-quebec.qc.ca/project/" +
            //    submissionProjectId +
            //    "/" +
            //    "token")
            //};

            //HttpResponseMessage response4 = await httpClient.SendAsync(request4);

            //var dTokenData = await response4.Content.ReadAsStringAsync();

            //dynamic dTokenObj = JsonConvert.DeserializeObject<dynamic>(dTokenData);

            //dynamic dToken = dTokenObj.key;

            //string downloadToken  = dToken.ToString();

            //string downloadURL = baseUrl +
            //    "form/" +
            //    submissionFormId +
            //    "/submission/" +
            //    submissionId +
            //    "/download?token=" +
            //    downloadToken;

            //HttpRequestMessage request5 = new HttpRequestMessage()
            //{
                
            //    Method = HttpMethod.Get,
            //    RequestUri = new Uri(downloadURL)
                
            //};

            //HttpResponseMessage response5 = await httpClient.SendAsync(request5);

            //var aa = await response5.Content.ReadAsStreamAsync();

            //return new FileStreamResult(aa, "application/pdf");


            return new OkObjectResult(adminToken);
        }
    }

    public class loginRequest
    {
        public loginObject data { get; set; }
    }

    public class loginObject
    {
        public string email { get; set; }
        public string password { get; set; }
    }
    [JsonArrayAttribute("submissionCollection")]
    public class submissionCollection
    {
        public List<submissionObj> submissions { get; set; }
    }

    public class submissionObj
    {
        public string _id { get; set; }
        public string form { get; set; }
        public string project { get; set; }
      
    }
}
