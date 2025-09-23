using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace Presentation.Controller;

public class ImageController : ApiController
{
    // GET /Image/7up.png
    [HttpGet("image/{fileName}")]
    public IActionResult GetImage(string fileName)
    {
        //"C:\Users\Hp\Desktop\BookStore Borrowing System\UI Design\Book Images"
        // Define the folder path where your image is located.
        string folderPath = @"C:\Users\Hp\Desktop\BookStore Borrowing System\UI Design\Book Images\";
        string filePath = Path.Combine(folderPath, fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("Image not found.");
        }

        // Determine the MIME type using the file extension.
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(filePath, out var contentType))
        {
            contentType = "application/octet-stream"; // fallback if unknown
        }

        // Read the file as bytes and return it with the correct Content-Type.
        var fileBytes = System.IO.File.ReadAllBytes(filePath);
        return File(fileBytes, contentType);
    }
}
