using Microsoft.AspNetCore.Mvc;
using TestGenerator.Web.Repositories;
using TestGenerator.Web.Services;

namespace TestGenerator.Web.Controllers
{
    public class TestGeneratorController : Controller
    {
        private readonly ITestRepository _testRepository;
        private readonly IFileProcessor _fileProcessor;

        public TestGeneratorController(ITestRepository testRepository, IFileProcessor fileProcessor)
        {
            _testRepository = testRepository;
            _fileProcessor = fileProcessor;
        }

        public IActionResult Index()
        {
            return View();
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
