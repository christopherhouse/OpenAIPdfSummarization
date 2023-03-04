using System;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using OpenAIPdfSummarization.Models;

namespace OpenAIPdfSummarization.Functions.Activities;

public class ExtractPdfTextActivity
{
    private static readonly DocumentAnalysisClient _documentAnalysisClient = new DocumentAnalysisClient(
        new Uri(Environment.GetEnvironmentVariable("cognitiveServicesEndpoint")),
        new AzureKeyCredential(Environment.GetEnvironmentVariable("cognitiveServicesKey")));

    [FunctionName(nameof(ExtractPdfTextActivity))]
    public async Task<PdfText> ExtractPdfText([ActivityTrigger] Uri blobSasUri)
    {
        var pdfText = new PdfText();

        var operation =
            await _documentAnalysisClient.AnalyzeDocumentFromUriAsync(WaitUntil.Completed, "prebuilt-read", blobSasUri);

        foreach (var page in operation.Value.Pages)
        {
            var builder = new StringBuilder();

            foreach (var line in page.Lines)
            {
                builder.AppendLine(line.Content);
            }

            pdfText.Pages.Add(builder.ToString());
        }

        return pdfText;
    }
}