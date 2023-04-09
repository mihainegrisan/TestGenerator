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

        public async Task<IActionResult> Index()
        {
            var tests = await _testRepository.GetTests();
            return View(tests);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Test test)
        {
            if (!ModelState.IsValid)
            {
              return View(test);
            }

            ViewBag.NumberOfQuestions = test.NumberOfQuestions;
            ViewBag.NumberOfAnswersPerQuestion = test.NumberOfAnswersPerQuestion;

            return View(nameof(AddTest));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTest(Test test)
        {
            if (!ModelState.IsValid)
            {
                return View(test);
            }

            await _testRepository.AddTest(test);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var test = await _testRepository.GetTest(id);

            return View(test);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var test = await _testRepository.GetTest(id);

            return View(test);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Test test)
        {
            if (!ModelState.IsValid)
            {
                return View(test);
            }

            await _testRepository.UpdateTest(test);

            return RedirectToAction(nameof(Index));
        }

        // GET: Test/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = _testRepository.Find(id);

            return View(test);
        }

        // POST: Test/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _testRepository.DeleteTestAsync(id);

            return RedirectToAction("Index");
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
