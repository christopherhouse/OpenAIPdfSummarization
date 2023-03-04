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
        MaxTokens = maxTokens;
    }

    [JsonProperty("max_tokens")]
    public int MaxTokens { get; }

    [JsonProperty("prompt")]
    public string Prompt { get; }

    public string ToJsonString()
    {
        var json = JsonConvert.SerializeObject(this);
        return json;
    }
}