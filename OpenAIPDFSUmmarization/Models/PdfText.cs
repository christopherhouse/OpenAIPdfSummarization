using System.Collections.Generic;
using System.Text;

namespace OpenAIPdfSummarization.Models;

public class PdfText
{
    public PdfText()
    {
        Pages = new List<string>();
    }

    public List<string> Pages { get; }

    public string GetPdfText()
    {
        var builder = new StringBuilder();

        foreach (var page in Pages)
        {
            builder.AppendLine(page);
        }

        return builder.ToString();
    }

    public string GetChatPrompt()
    {
        return $"Please summarize this text: {GetPdfText()}";
    }

    public string GetTlDrPrompt()
    {
        var builder = new StringBuilder();
        builder.AppendLine(GetPdfText());
        builder.AppendLine("tl;dr;");

        return GetChatPrompt();
    }
}