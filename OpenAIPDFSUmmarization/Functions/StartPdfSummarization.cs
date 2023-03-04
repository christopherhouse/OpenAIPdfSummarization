using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using OpenAIPdfSummarization.Models;

namespace OpenAIPdfSummarization.Functions;

public class StartPdfSummarization
{
    [FunctionName(nameof(StartPdfSummarization))]
    public static async Task<IActionResult> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req,
        [DurableClient] IDurableOrchestrationClient starter,
        ILogger log)
    {
        var formData = await req.ReadFormAsync();
        var file = formData.Files["file"];
        var fileName = file.FileName;
        byte[] fileBytes;

        using (var memory = new MemoryStream())
        {
            await file.CopyToAsync(memory);
            fileBytes = memory.ToArray();
        }

        var fileData = new FileData
        {
            Data = fileBytes,
            FileName = fileName,
            Size = fileBytes.Length
        };

        var instanceId = await starter.StartNewAsync(nameof(PdfSummarizationOrchestrator), fileData);

        return starter.CreateCheckStatusResponse(req, instanceId);
    }
}