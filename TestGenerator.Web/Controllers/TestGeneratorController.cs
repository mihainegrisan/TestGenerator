using Microsoft.AspNetCore.Mvc;
using TestGenerator.DAL.Models;
using TestGenerator.Web.Repositories;
using TestGenerator.Web.Services;

namespace TestGenerator.Web.Controllers;

public class TestGeneratorController : Controller
{
    private readonly IChatGptClient _chatGptClient;
    private readonly IFileProcessor _fileProcessor;
    private readonly ITestRepository _testRepository;

    public TestGeneratorController(ITestRepository testRepository, IFileProcessor fileProcessor,
        IChatGptClient chatGptClient)
    {
        _testRepository = testRepository;
        _fileProcessor = fileProcessor;
        _chatGptClient = chatGptClient;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var tests = await _testRepository.GetTests();
        return View(tests);
    }

    [HttpGet]
    public async Task<IActionResult> Generate()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(Test test, IFormFile file)
    {
        if (!ModelState.IsValid)
        {
            return View(test);
        }

        var chatMessage = await _fileProcessor.GetTextFromFileAsync(file);

        var responseMessage = await _chatGptClient.SendChatMessage(test, chatMessage);

        _chatGptClient.PopulateTestWithApiResponse(test, responseMessage);

        TempData["Test"] = test;

        return View(nameof(GenerateTest), test);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateTest(Test test)
    {
        //bool isUploaded = await _fileProcessor.UploadFile(file);

        //if (!isUploaded)
        //{
        //  return View();
        //}

        //TempData["Message"] = "File uploaded successfully";
        // From now on, work with the saved file

        if (!ModelState.IsValid)
        {
            return View(test);
        }

        await _testRepository.AddTest(test);

        var tests = await _testRepository.GetTests();

        return View(nameof(Index), tests);
    }
}