using System.Collections.Generic;
using Newtonsoft.Json;

namespace OpenAIPdfSummarization.Models;

public class SummarizationResult
{
    public SummarizationResult()
    {
        PageSummaries = new List<string>();
    }

    [JsonProperty("totalTokens")]
    public int TotalTokens { get; set; }

    [JsonProperty("combinedSummaryTokens")]
    public int CombinedSummaryTokens { get; set; }

    [JsonProperty("summary")]
    public string Summary { get; set; }

    [JsonProperty("pageSummaries")]
    public List<string> PageSummaries { get; }
}