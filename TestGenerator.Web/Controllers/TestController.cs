using Microsoft.AspNetCore.Mvc;
using TestGenerator.DAL.Models;
using TestGenerator.Web.Repositories;
using TestGenerator.Web.Services;

namespace TestGenerator.Web.Controllers
{
    public class TestController : Controller
    {
        private readonly ITestRepository _testRepository;
        private readonly IFileProcessor _fileProcessor;

        public TestController(ITestRepository testRepository, IFileProcessor fileProcessor)
        {
            _testRepository = testRepository;
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
                _testRepository.AddTest(test);

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
