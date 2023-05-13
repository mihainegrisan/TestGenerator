using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestGenerator.DAL.Models;
using TestGenerator.DAL.Repositories;
using TestGenerator.Web.Services;
using TestGenerator.Web.Utility;

namespace TestGenerator.Web.Controllers;

[Authorize]
public class TestController : Controller
{
    private readonly IFileProcessor _fileProcessor;
    private readonly INotyfService _notifyService;
    private readonly ITestRepository _testRepository;
    private readonly UserManager<IdentityUser> _userManager;

    public TestController(
        UserManager<IdentityUser> userManager,
        ITestRepository testRepository,
        IFileProcessor fileProcessor,
        INotyfService notifyService)
    {
        _userManager = userManager;
        _testRepository = testRepository;
        _fileProcessor = fileProcessor;
        _notifyService = notifyService;
    }

    public async Task<IActionResult> Index(
        string? sortOrder,
        string? currentFilter,
        string? searchString,
        int? pageNumber,
        int? pageSize)
    {
        ViewData["CurrentSort"] = sortOrder;
        ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewData["DateSortParam"] = sortOrder == "date" ? "date_desc" : "date";
        ViewData["PageSize"] = pageSize;

        if (searchString != null)
        {
            pageNumber = 1;
        }
        else
        {
            searchString = currentFilter;
        }

        ViewData["CurrentFilter"] = searchString;

        var currentUser = await _userManager.GetUserAsync(User);

        var tests = _testRepository.GetTests(sortOrder, searchString, false, currentUser.Id);

        return View(await PaginatedList<Test>.CreateAsync(tests, pageNumber ?? 1, pageSize ?? 10));
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
        ViewBag.NumberOfQuestions = test.NumberOfQuestions;
        ViewBag.NumberOfAnswersPerQuestion = test.NumberOfAnswersPerQuestion;

        if (!ModelState.IsValid)
        {
            return View(test);
        }

        foreach (var question in test.Questions)
        {
            if (question.Answers.Any(a => a.IsCorrect))
            {
                continue;
            }

            TempData["ErrorMessage"] = "Please select at least one correct answer per each question.";

            return View(test);
        }

        test.IsCreatedManually = true;
        test.IsAutoCreatedFromQuestions = false;
        test.IsAutoCreatedByChatGpt = false;

        var currentUser = await _userManager.GetUserAsync(User);
        if (currentUser != null)
        {
            test.Author = currentUser;
            test.AuthorId = currentUser.Id;
            test.CreatedAt = DateTime.Now;
            test.EditedAt = DateTime.Now;
        }
        else
        {
            throw new Exception("User not found.");
        }

        foreach (var question in test.Questions)
        {
            question.CreatedAt = DateTime.Now;
            question.EditedAt = DateTime.Now;
            question.Author = currentUser;
            question.AuthorId = currentUser.Id;
        }

        await _testRepository.AddTestAsync(test);

        _notifyService.Success("Test Created!");

        return View(nameof(Details), test);
    }

    public async Task<IActionResult> DownloadPdf(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var test = await _testRepository.GetTestAsync(id, currentUser.Id);

        var stream = _fileProcessor.GeneratePdf(test);

        stream.Position = 0;

        return File(stream, "application/pdf", $"{test.Name}.pdf");
    }

    public async Task<IActionResult> DownloadWord(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var test = await _testRepository.GetTestAsync(id, currentUser.Id);

        var stream = _fileProcessor.GenerateWord(test);

        stream.Position = 0;

        return File(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document", $"{test.Name}.docx");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var test = await _testRepository.GetTestAsync(id, currentUser.Id);

        return View(test);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Test test)
    {
        if (!ModelState.IsValid)
        {
            return View(test);
        }

        var currentUser = await _userManager.GetUserAsync(User);

        test.Author = currentUser;
        test.AuthorId = currentUser.Id;
        
        await _testRepository.UpdateTestAsync(test, currentUser.Id);

        _notifyService.Success("Test Updated!");

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var currentUser = await _userManager.GetUserAsync(User);

        var test = await _testRepository.GetTestAsync(id, currentUser.Id);

        return View(test);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, bool? deleteQuestions)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var test = await _testRepository.GetTestAsync(id, currentUser.Id);

        await _testRepository.DeleteTestAsync(test.TestId, deleteQuestions, currentUser.Id);

        _notifyService.Success("Test Deleted!");

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var currentUser = await _userManager.GetUserAsync(User);

        var test = await _testRepository.GetTestAsync(id, currentUser.Id);

        return View(test);
    }
}