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
    public static async Task<string> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var fileData = context.GetInput<FileData>();

        var blobSasUri = await context.CallActivityAsync<Uri>(nameof(StorePdfBlobActivity), fileData);
        var pdfText = await context.CallActivityAsync<PdfText>(nameof(ExtractPdfTextActivity), blobSasUri);
        var summary = await context.CallActivityAsync<string>(nameof(SummarizeTextActivity), pdfText);

        return summary;
    }

}