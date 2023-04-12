using Microsoft.AspNetCore.Mvc;
using TestGenerator.Web.Repositories;
using TestGenerator.Web.Services;

namespace TestGenerator.Web.Controllers
{
    public class TestGeneratorController : Controller
    {
        private readonly ITestRepository _testRepository;
        private readonly IFileProcessor _fileProcessor;
        private readonly IChatGptClient _chatGPTClient;

        public TestGeneratorController(ITestRepository testRepository, IFileProcessor fileProcessor, IChatGptClient chatGPTClient)
        {
            _testRepository = testRepository;
            _fileProcessor = fileProcessor;
            _chatGPTClient = chatGPTClient;
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
        public async Task<IActionResult> Generate(IFormFile file)
        {
            //bool isUploaded = await _fileProcessor.UploadFile(file);

            //if (!isUploaded)
            //{
            //  return View();
            //}

            //TempData["Message"] = "File uploaded successfully";
            // From now on, work with the saved file

            var text = await _fileProcessor.GetTextFromFileAsync(file);

            var response = await _chatGPTClient.SendMessage(text, maxChunkSize: 250);

            return await Task.FromResult<IActionResult>(View(response));
        }
    }
}
