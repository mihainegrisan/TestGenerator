using Microsoft.AspNetCore.Mvc;
using TestGenerator.DAL.Data;
using TestGenerator.DAL.Models;
using TestGenerator.Web.Services;

namespace TestGenerator.Web.Controllers
{
  public class TestController : Controller
  {
    private readonly ApplicationDbContext _context;
    private readonly IFileProcessor _fileProcessor;

    public TestController(ApplicationDbContext context, IFileProcessor fileProcessor)
    {
      _context = context;
      _fileProcessor = fileProcessor;
    }

    // GET: Test/Create
    public IActionResult Create()
    {
      return View();
    }

    // POST: Test/AddTest
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Add(Test test)
    {
      if (ModelState.IsValid)
      {
        _context.Tests.Add(test);
        _context.SaveChanges();
        return RedirectToAction("Index");
      }

      return View(test);
    }


    // GET: Test/Generate
    public IActionResult Generate()
    {
      return View();
    }

    [HttpPost]
    public Task<IActionResult> Generate(IFormFile file)
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
