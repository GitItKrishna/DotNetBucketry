using DotNetBucketry.Core.Communication.Bucket;

namespace DotNetBucketry.Core.Interfaces;

public interface IBucketRepository
{
    Task<bool> DoesBucketExists(string bucketName);
    Task<CreateBucketResponse> CreateBucket(string bucketName);
    Task<IEnumerable<ListS3BucketResponse>> ListBuckets();
}