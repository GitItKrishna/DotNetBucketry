using DotNetBucketry.Core.Communication.Files;
using DotNetBucketry.Core.Interfaces;
using System.Collections;
using Amazon.S3;
using Amazon.S3.Model;
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
    public async Task<AddFileResponse> UploadFiles(string bucketName, IList<IFormFile> formFiles)
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

            var urlExpiryRequest = new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Key = file.FileName,
                Expires = DateTime.Now.AddHours(1)
            };
            var url = _s3Client.GetPreSignedURL(urlExpiryRequest);
            response.Add(url);
        }
        return new AddFileResponse
        {
            PreSignedUrls = response
        };
    }
    public async Task<IEnumerable<ListFilesResponse>> ListFiles(string bucketName)
    {
        var response = await _s3Client.ListObjectsAsync(bucketName);
        var listObjectsResponse = response.S3Objects.Select(b=> new ListFilesResponse
        {
            BucketName = b.BucketName,
            Key = b.Key,
            Owner = b.Owner.DisplayName,
            Size = b.Size
        });
        return listObjectsResponse;
    }
    public async Task DownloadFile(string BucketName, string fileName)
    {
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var downloadPath = Path.Combine(desktopPath, "DownloadedFiles");
        
        if(string.IsNullOrEmpty(BucketName) || string.IsNullOrEmpty(fileName))
        {
            throw new ArgumentNullException("BucketName and fileName are required");
        }
        var downloadRequest = new TransferUtilityDownloadRequest
        {
            BucketName = BucketName,
            Key = fileName,
            FilePath = Path.Combine(downloadPath, fileName)
        };
        using(var fileTransferUtility = new TransferUtility(_s3Client))
        {
            await fileTransferUtility.DownloadAsync(downloadRequest);
        }
    }
    public async Task<DeleteFileResponse> DeleteFile(string bucketName, string fileName)
    {
        if(string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(fileName))
        {
            throw new ArgumentNullException("BucketName and fileName are required");
        }
        var deleteRequest = new DeleteObjectsRequest
        {
            BucketName = bucketName
        };
        deleteRequest.AddKey(fileName);
        var response = await _s3Client.DeleteObjectsAsync(deleteRequest);
        return new DeleteFileResponse
        {
            TotalDeletedFiles = response.DeletedObjects.Count
        };
    }
}   