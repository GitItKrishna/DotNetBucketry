using DotNetBucketry.Core.Communication.Files;
using Microsoft.AspNetCore.Http;

namespace DotNetBucketry.Core.Interfaces;

public interface IFilesRepository
{
    Task<bool> UploadFiles(string bucketName, IList<IFormFile> formFiles);
}