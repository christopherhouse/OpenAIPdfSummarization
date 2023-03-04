using System.Collections.Generic;
using System.Text;

namespace OpenAIPdfSummarization.Models;

public class PageSummaries
{
    public PageSummaries()
    {
        Summaries = new List<string>();
    }

    public List<string> Summaries { get; }

    public string GetSummariesAsString()
    {
        var builder = new StringBuilder();
        foreach (var summary in Summaries)
        {
            builder.AppendLine(summary);
        }

        return builder.ToString();
    }
}