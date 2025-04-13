namespace DotNetBucketry.Core.Communication.Bucket;

public class ListS3BucketResponse
{
    public string BucketName { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
}