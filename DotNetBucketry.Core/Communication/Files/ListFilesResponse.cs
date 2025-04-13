namespace DotNetBucketry.Core.Communication.Files;

public class ListFilesResponse
{
    public string BucketName { get; set; } = string.Empty;
    public string Key { get; set; }=string.Empty;
    public string Owner { get; set; }=string.Empty;
    public long Size { get; set; }
}