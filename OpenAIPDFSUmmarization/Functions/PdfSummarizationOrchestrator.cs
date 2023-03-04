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
    public static async Task<SummarizationResult> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var fileData = context.GetInput<FileData>();

        var blobSasUri = await context.CallActivityAsync<Uri>(nameof(StorePdfBlobActivity), fileData);
        var pdfText = await context.CallActivityAsync<PdfText>(nameof(ExtractPdfTextActivity), blobSasUri);

        var pageSummaries = new PageSummaries();

        foreach (var page in pdfText.Pages)
        {
            var summary = await context.CallActivityAsync<string>(nameof(SummarizeTextActivity), page);
            pageSummaries.Summaries.Add(summary);
        }

        var combinedSummaries = pageSummaries.GetSummariesAsString();
        var finalSummary = await context.CallActivityAsync<string>(nameof(SummarizeTextActivity), combinedSummaries);

        var result = new SummarizationResult
        {
            CombinedSummaryTokens = combinedSummaries.Length,
            Summary = finalSummary,
            TotalTokens = pdfText.GetPdfText().Length
        };

        result.PageSummaries.AddRange(pageSummaries.Summaries);

        return result;
    }

}