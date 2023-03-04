namespace OpenAIPdfSummarization.Models;

public class FileData
{
    public string FileName { get; set; }

    public long Size { get; set; }

    public byte[] Data { get; set; }
}