using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using OpenAIPdfSummarization.Functions.Activities;
using OpenAIPdfSummarization.Models;

namespace OpenAIPdfSummarization.Functions;

public class PdfSummarizationOrchestrator
{
    [FunctionName(nameof(PdfSummarizationOrchestrator))]
    public async Task<SummarizationResult> RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var fileData = context.GetInput<FileData>();
        context.SetCustomStatus(GetCustomStatus(false,
            false, 
            0,
            0,
            false))
            ;
        var blobSasUri = await context.CallActivityAsync<Uri>(nameof(StorePdfBlobActivity), fileData);
        context.SetCustomStatus(GetCustomStatus(true, 
            false,
            0,
            0,
            false));

        var pdfText = await context.CallActivityAsync<PdfText>(nameof(ExtractPdfTextActivity), blobSasUri);
        context.SetCustomStatus(GetCustomStatus(true,
            true,
            0,
            pdfText.Pages.Count,
            false));

        var pageSummaries = new PageSummaries();

        var counter = 0;
        foreach (var page in pdfText.Pages)
        {
            counter++;
            var summary = await context.CallActivityAsync<string>(nameof(SummarizeTextActivity), page);
            context.SetCustomStatus(GetCustomStatus(true,
                true,
                counter,
                pdfText.Pages.Count,
                false));
            pageSummaries.Summaries.Add(summary);

        }

        var combinedSummaries = pageSummaries.GetSummariesAsString();
        var finalSummary = await context.CallActivityAsync<string>(nameof(SummarizeTextActivity), combinedSummaries);
        context.SetCustomStatus(GetCustomStatus(true,
            true,
            counter,
            pdfText.Pages.Count,
            true));

        var result = new SummarizationResult
        {
            CombinedSummaryTokens = combinedSummaries.Length,
            Summary = finalSummary,
            TotalTokens = pdfText.GetPdfText().Length
        };

        result.PageSummaries.AddRange(pageSummaries.Summaries);

        return result;
    }

    private string GetCustomStatus(bool blobCompleted,
        bool pdfExtractionCompleted,
        int numberOfPagesSummarized,
        int totalNumberOfPages,
        bool pageSummariesSummarized)
    {
        var customStatus = $"PDF Extraction complete: [{(pdfExtractionCompleted ? "X" : " ")}], Pages Summarized: [{numberOfPagesSummarized}/{totalNumberOfPages}], Page Summaries Summarized: [{(pageSummariesSummarized ? "X" : " ")}]";
        
        return customStatus ;
    }
}