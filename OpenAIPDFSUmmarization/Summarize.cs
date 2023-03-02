using System;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace OpenAIPdfSummarization
{
    public static class Summarize
    {
        private static readonly AzureKeyCredential _credentials =
            new AzureKeyCredential(Environment.GetEnvironmentVariable("cognitiveServicesKey"));

        private static readonly Uri
            _endpoint = new Uri(Environment.GetEnvironmentVariable("cognitiveServicesEndpoint"));

        [FunctionName("Summarize")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {

        }
    }
}
