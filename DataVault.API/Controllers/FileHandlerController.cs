using DataVault.API.Logic;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace DataVault.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileHandlerController : Controller
{

    private readonly FileHandlerService _fileService;

    public FileHandlerController(FileHandlerService fileService)
    {
        _fileService = fileService;
    }

    [HttpGet]
    public async Task<IActionResult> ListAllBlobs()
    {
        var result = await _fileService.ListAsync();
        return Ok(result);
    }
    
    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        var result = await _fileService.UploadAsync(file);
        return Ok(result);
    }

    [HttpGet]
    [Route("fileName")]
    public async Task<IActionResult> Download(string filename)
    {
        var result = await _fileService.DownloadAsync(filename);
        return File(result.Content, result.ContentType, result.Name);
    }

    [HttpDelete]
    [Route("filename")]
    public async Task<IActionResult> Delete(string filename)
    {
        var result = await _fileService.DeleteAsync(filename);
        return Ok(result);
    }
    
    
}