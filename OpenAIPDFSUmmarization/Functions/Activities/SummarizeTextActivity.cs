using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Azure;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Newtonsoft.Json.Linq;
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
    public async Task<string> SummarizeText([ActivityTrigger] string text)
    {
        string output;
        var pageSummaries = new List<string>();
        var uri = new Uri(
            $"{_openAIEndpoint}/openai/deployments/{_openAIDeploymentName}/completions?api-version=2022-12-01");

        var prompt = new ChatPrompt(text, 4097);
        var requestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = uri,
            Content = new StringContent(prompt.ToJsonString(), Encoding.UTF8, "application/json")
        };

        if (prompt.Tokens > 4097)
        {
            throw new InvalidOperationException();
        }

        requestMessage.Headers.Add("api-key", _openAIKey);

        var response = await _httpClient.SendAsync(requestMessage);

        if (response.IsSuccessStatusCode)
        {
            var jsonString = await response.Content.ReadAsStringAsync();
            var jObject = JObject.Parse(jsonString);
            output = (string)jObject["choices"][0]["text"];
        }
        else
        {
            var message = await response.Content.ReadAsStringAsync();
            output = $"API call failed with status {response.StatusCode}, message: {message}";
        }

        return output;
    }
}