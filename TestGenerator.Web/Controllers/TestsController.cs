using Microsoft.AspNetCore.Mvc;
using TestGenerator.DAL.Data;

namespace TestGenerator.Web.Controllers
{
  public class TestsController : Controller
  {
    private readonly ApplicationDbContext _context;

    public TestsController(ApplicationDbContext context)
    {
      _context = context;
    }

    // GET: 
    public IActionResult GenerateTest()
    {
      return View();
    }

    [HttpPost]
    public async Task<IActionResult> GenerateTest(IFormFile file)
    {
      bool isUploaded;
      isUploaded = await FileUpload(file);

      if (!isUploaded)
      {
        return View();
      }

      TempData["Message"] = "File uploaded successfully";

      return View();
    }

    private async Task<bool> FileUpload(IFormFile file)
    {
      bool isCopied = false;
      var allowedExtensions = new[] { ".doc", ".docx" };

      try
      {
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(fileExtension))
        {
          TempData["Message"] = $"File with {fileExtension} extension not allowed! Please add a .doc or .docx file.";
          return false;
        }

        if (file.Length <= 0)
        {
          TempData["Message"] = "File is empty";
          return false;
        }
        
        string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "Uploaded"));

        await using (var fileStream = new FileStream(Path.Combine(path, filename), FileMode.Create))
        {
          await file.CopyToAsync(fileStream);
        }

        isCopied = true;
      }
      catch (Exception ex)
      {
        TempData["Message"] = $"An unknown error occurred! Please try again. If the problem persists contact support.";
        //throw new Exception($"Message {ex.Message}");
      }

      return isCopied;
    }


  }
}
