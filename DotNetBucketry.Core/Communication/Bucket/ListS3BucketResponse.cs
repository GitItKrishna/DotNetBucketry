namespace DotNetBucketry.Core.Communication.Bucket;

public class ListS3BucketResponse
{
    public string BucketName { get; set; }
    public DateTime CreationDate { get; set; }
}