using DotNetBucketry.Core.Communication.Files;
using DotNetBucketry.Core.Interfaces;
using System.Collections;
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.AspNetCore.Http;

namespace DotNetBucketry.Infrastructure.Repositories;

public class FilesRepository : IFilesRepository
{
    private readonly IAmazonS3 _s3Client;
    public FilesRepository(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }
    public async Task<bool> UploadFiles(string bucketName, IList<IFormFile> formFiles)
    {
        var response = new List<string>();
        foreach (var file in formFiles)
        {
            var uploadRequest = new TransferUtilityUploadRequest
            {
                InputStream = file.OpenReadStream(),
                Key = file.FileName,
                BucketName = bucketName,
                CannedACL = S3CannedACL.NoACL
            };
            using(var fileTransferUtility = new TransferUtility(_s3Client))
            {
                await fileTransferUtility.UploadAsync(uploadRequest);
            }
        }
        return true;
    }
}