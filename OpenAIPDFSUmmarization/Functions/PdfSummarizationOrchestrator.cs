using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using OpenAIPdfSummarization.Functions.Activities;
using OpenAIPdfSummarization.Models;

namespace OpenAIPdfSummarization.Functions;

public static class PdfSummarizationOrchestrator
{
    [FunctionName(nameof(PdfSummarizationOrchestrator))]
    public static async Task<List<string>> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var fileData = context.GetInput<FileData>();

        var outputs = new List<string>();

        //// Replace "hello" with the name of your Durable Activity Function.
        //outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Tokyo"));
        //outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "Seattle"));
        //outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello), "London"));

        var blobSasUri = await context.CallActivityAsync<Uri>(nameof(StorePdfBlobActivity), fileData);
        var pdfText = await context.CallActivityAsync<PdfText>(nameof(ExtractPdfTextActivity), blobSasUri);

        outputs.Add(blobSasUri.ToString());

        //// returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
        return outputs;
    }

}