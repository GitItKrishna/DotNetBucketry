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
}