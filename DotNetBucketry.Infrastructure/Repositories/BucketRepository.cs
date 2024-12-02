using Amazon.S3;
using Amazon.S3.Model;
using DotNetBucketry.Core.Communication.Bucket;
using DotNetBucketry.Core.Interfaces;

namespace DotNetBucketry.Infrastructure.Repositories;

public class BucketRepository : IBucketRepository
{
    private readonly IAmazonS3 _s3CLient;
    public BucketRepository(IAmazonS3 s3CLient)
    {
        _s3CLient = s3CLient;
    }
    public async Task<bool> DoesBucketExists(string bucketName)
    {
        return await _s3CLient.DoesS3BucketExistAsync(bucketName);
    }

    public async Task<CreateBucketResponse> CreateBucket(string bucketName)
    {
        var putBucketRequest = new PutBucketRequest
        {
            BucketName = bucketName
        };
        var response = await _s3CLient.PutBucketAsync(putBucketRequest);
        return new CreateBucketResponse
        {
            BucketName = bucketName,
            RequestId = response.ResponseMetadata.RequestId
        };
    }
    
    public async Task<IEnumerable<ListS3BucketResponse>> ListBuckets()
    {
        var response = await _s3CLient.ListBucketsAsync();
        return response.Buckets.Select(b => new ListS3BucketResponse
        {
            BucketName = b.BucketName,
            CreationDate = b.CreationDate
        });
    }
}