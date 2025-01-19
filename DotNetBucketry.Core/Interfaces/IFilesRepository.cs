using Amazon.S3.Model;
using DotNetBucketry.Core.Communication.Files;
using Microsoft.AspNetCore.Http;

namespace DotNetBucketry.Core.Interfaces;

public interface IFilesRepository
{
    Task<AddFileResponse> UploadFiles(string bucketName, IList<IFormFile> formFiles);
    Task<IEnumerable<ListFilesResponse>> ListFiles(string bucketName);
    Task DownloadFile(string BucketName, string fileName);
    Task<DeleteFileResponse> DeleteFile(string bucketName, string fileName);
    Task UploadFilesLowLevelAPI(string bucketName, IFormFile formFiles);
    Task<GetObjectResponse> GetFileForDownloadAsync(string bucketName, string bucketKey, CancellationToken cancellationToken);
    
}