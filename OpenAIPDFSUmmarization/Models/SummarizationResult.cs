using System.Collections.Generic;

namespace OpenAIPdfSummarization.Models;

public class SummarizationResult
{
    public SummarizationResult()
    {
        PageSummaries = new List<string>();
    }

    public int TotalTokens { get; set; }

    public int CombinedSummaryTokens { get; set; }

    public string Summary { get; set; }

    public List<string> PageSummaries { get; }
}