using Microsoft.AspNetCore.Mvc;
using TestGenerator.DAL.Data;
using TestGenerator.Web.Services;

namespace TestGenerator.Web.Controllers
{
  public class TestsController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly IFileProcessor _fileProcessor;

    public TestsController(ApplicationDbContext context, IFileProcessor fileProcessor)
    {
      _context = context;
      _fileProcessor = fileProcessor;
    }

    // GET: 
    public IActionResult GenerateTest()
    {
      return View();
    }

    [HttpPost]
    public Task<IActionResult> GenerateTest(IFormFile file)
    {
      //bool isUploaded = await _fileProcessor.UploadFile(file);

      //if (!isUploaded)
      //{
      //  return View();
      //}

      //TempData["Message"] = "File uploaded successfully";
      // From now on, work with the saved file

      var text = _fileProcessor.GetTextFromFile(file);

      return Task.FromResult<IActionResult>(View());
    }
  }
}
