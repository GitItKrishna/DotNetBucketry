using System.Net.Mime;
using DotNetBucketry.Core.Communication.Files;
using DotNetBucketry.Core.Interfaces;

namespace DotNetBucketry.Controllers;
using Microsoft.AspNetCore.Mvc;

[Route("api/files")]
[ApiController]
public class FilesController : ControllerBase
{
    private readonly IFilesRepository _filesRepository;
    public FilesController(IFilesRepository filesRepository)
    {
        _filesRepository = filesRepository;
    }
    [HttpPost]
    [Route("upload-low/{bucketName}")]
    public async Task UploadFile(string bucketName, IFormFile formFile)
    {
        if (string.IsNullOrWhiteSpace(bucketName) || formFile == null)
        {
            BadRequest("Bucket name and file are required");
        }
        await _filesRepository.UploadFilesLowLevelAPI(bucketName,  formFile);
    }
    
    
    [HttpPost]
    [Route("upload/{bucketName}")]
    public async Task<ActionResult<AddFileResponse>> UploadFiles(string bucketName, IList<IFormFile> formFiles)
    {
        if (string.IsNullOrWhiteSpace(bucketName) || formFiles == null)
        {
            return BadRequest("Bucket name and files are required");
        }
        var response = await _filesRepository.UploadFiles(bucketName, formFiles);
        return Ok(response);
    }
    
    [HttpGet]
    [Route("list/{bucketName}")]
    public async Task<ActionResult<IEnumerable<ListFilesResponse>>> ListFiles(string bucketName)
    {
        if (string.IsNullOrWhiteSpace(bucketName))
        {
            return BadRequest("Bucket name is required");
        }
        var response = await _filesRepository.ListFiles(bucketName);
        return Ok(response);
    }
    [HttpGet]
    [Route("{bucketName}/download/{fileName}")]
    public async Task<ActionResult> DownloadFile(string bucketName, string fileName)
    {
        if (string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(fileName))
        {
            return BadRequest("Bucket name and file name are required");
        }
        await _filesRepository.DownloadFile(bucketName, fileName);
        return Ok();
    }
    [HttpDelete]
    [Route("{bucketName}/delete/{fileName}")]
    public async Task<ActionResult<DeleteFileResponse>> DeleteFile(string bucketName, string fileName)
    {
        if (string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(fileName))
        {
            return BadRequest("Bucket name and file name are required");
        }
        var response = await _filesRepository.DeleteFile(bucketName, fileName);
        return Ok(response);
    }
    [HttpGet]
    [Route("{bucketName}/download-low/{fileName}")]
    public async Task<ActionResult> DownloadFileLowLeveAPI(string bucketName, string fileName)
    {
        if (string.IsNullOrWhiteSpace(bucketName) || string.IsNullOrWhiteSpace(fileName))
        {
            return BadRequest("Bucket name and file name are required");
        }   
        var getObjectResponse =  await _filesRepository.GetFileForDownloadAsync(bucketName, fileName, new CancellationToken());
       
        if (getObjectResponse.ResponseStream == null)
        {
            return new NotFoundResult();
        }
        var memoryStream = new MemoryStream();
        await getObjectResponse.ResponseStream.CopyToAsync(memoryStream);
        if (memoryStream.Length == 0)
        {
            return new NotFoundResult();
        }

        memoryStream.Position = 0; // Reset the position to the beginning of the stream

        var contentType = getObjectResponse.Headers.Keys.Contains("Content-Type")
            ? getObjectResponse.Headers["Content-Type"]
            : MediaTypeNames.Application.Octet;

        return new FileStreamResult(getObjectResponse.ResponseStream, contentType)
        {
            FileDownloadName = fileName
        };
    }
}