using DropBeatAPI.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace DropBeatAPI.Web.Controllers
{
    [ApiController]
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        //[HttpPost("upload-image")]
        //[Consumes("multipart/form-data")]
        //public async Task<ActionResult<string>> UploadImage([FromForm] UploadImageRequest request, [FromServices] YandexStorageService storageService)
        //{
        //    if (request.Image == null || request.Image.Length == 0)
        //    {
        //        return BadRequest("Файл не загружен.");
        //    }

        //    string imageUrl = await storageService.UploadFileAsync(request.Image.OpenReadStream(), request.Image.FileName, request.Image.ContentType);

        //    return Ok(new { ImageUrl = imageUrl });
        //}

        //public class UploadImageRequest
        //{
        //    public IFormFile Image { get; set; }
        //}
    }
}
