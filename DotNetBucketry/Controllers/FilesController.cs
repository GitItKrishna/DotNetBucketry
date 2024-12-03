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
}