using DotNetBucketry.Core.Communication.Bucket;
using DotNetBucketry.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNetBucketry.Controllers;

[Route("api/bucket")]
[ApiController]
public class BucketController : ControllerBase
{
    private readonly IBucketRepository _bucketRepository;
    public BucketController(IBucketRepository bucketRepository)
    {
        _bucketRepository = bucketRepository;
    }
    [HttpPost]
    [Route("create/{bucketName}")]
    public async Task<ActionResult<CreateBucketResponse>> CreateBucket([FromRoute]string bucketName)
    {
        if(string.IsNullOrWhiteSpace(bucketName))
        {
            return BadRequest();
        }
        var bucketExits = await _bucketRepository.DoesBucketExists(bucketName);
        if(bucketExits)
        {
            return BadRequest("S3 Bucket with the same name already exists");
        }
        var bucketCreationResult = await _bucketRepository.CreateBucket(bucketName);
        if (bucketCreationResult == null)
        {
            return BadRequest("Failed to create S3 Bucket");
        }
        return Ok(bucketCreationResult);
    }
    [HttpGet]
    [Route("list")]
    public async Task<ActionResult<IEnumerable<ListS3BucketResponse>>> ListBuckets()
    {
        var buckets = await _bucketRepository.ListBuckets();
        if (!buckets.Any() || buckets == null)
        {
            return NotFound();
        }
        return Ok(buckets);
    }

}