using DotNetBucketry.Core.Communication.Files;
using Microsoft.AspNetCore.Http;

namespace DotNetBucketry.Core.Interfaces;

public interface IFilesRepository
{
    Task<AddFileResponse> UploadFiles(string bucketName, IList<IFormFile> formFiles);
    Task<IEnumerable<ListFilesResponse>> ListFiles(string bucketName);
}