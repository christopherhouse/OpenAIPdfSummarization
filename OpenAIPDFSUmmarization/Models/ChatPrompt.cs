using Newtonsoft.Json;

namespace OpenAIPdfSummarization.Models;

public class ChatPrompt
{
    public ChatPrompt()
    {
    }

    public ChatPrompt(string text, int maxTokens)
    {
        Prompt = text;
        Tokens = text.Length > maxTokens ? maxTokens : text.Length;
    }

    [JsonProperty("max_tokens")]
    public int Tokens { get; }

    [JsonProperty("prompt")]
    public string Prompt { get; }

    public string ToJsonString()
    {
        var json = JsonConvert.SerializeObject(this);
        return json;
    }
}