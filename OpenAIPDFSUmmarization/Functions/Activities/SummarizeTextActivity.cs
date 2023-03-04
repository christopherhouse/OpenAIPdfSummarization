using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using OpenAIPdfSummarization.Models;

namespace OpenAIPdfSummarization.Functions.Activities;

public class SummarizeTextActivity
{
    private readonly HttpClient _httpClient;
    private readonly string _openAIEndpoint = Environment.GetEnvironmentVariable("openAIEndpoint");
    private readonly string _openAIKey = Environment.GetEnvironmentVariable("OpenAIKey");
    private readonly string _openAIDeploymentName = Environment.GetEnvironmentVariable("openAIDeployment");

    public SummarizeTextActivity(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }

    [FunctionName(nameof(SummarizeTextActivity))]
    public async Task<string> SummarizeText([ActivityTrigger] PdfText pdfText)
    {
        string output;
        var pageSummaries = new List<string>();
        var uri = new Uri(
            $"{_openAIEndpoint}/openai/deployments/{_openAIDeploymentName}/completions?api-version=2022-12-01");

        foreach (var page in pdfText.Pages)
        {
            page
        }


        var prompt = new ChatPrompt(pdfText.GetPdfText(), 4096);
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = uri,
            Content = new StringContent(prompt.ToJsonString(), Encoding.UTF8, "application/json")
        };

        requestMessage.Headers.Add("api-key", _openAIKey);

        var response = await _httpClient.SendAsync(requestMessage);

        if (response.IsSuccessStatusCode)
        {
            output = await response.Content.ReadAsStringAsync();
        }
        else
        {
            output = $"API call failed with status {response.StatusCode}";
        }

        return output;
    }
}