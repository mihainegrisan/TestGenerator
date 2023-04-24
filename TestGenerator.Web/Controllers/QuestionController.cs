using Microsoft.AspNetCore.Mvc;
using TestGenerator.DAL.Models;
using TestGenerator.Web.Repositories;

namespace TestGenerator.Web.Controllers;

public class QuestionController : Controller
{
    private readonly IQuestionRepository _questionRepository;
    private readonly ITestRepository _testRepository;

    public QuestionController(IQuestionRepository questionRepository, ITestRepository testRepository)
    {
        _questionRepository = questionRepository;
        _testRepository = testRepository;
    }

    public async Task<IActionResult> Index()
    {
        var questions = await _questionRepository.GetDistinctQuestionsAsync();

        // TODO: Add Pagination

        return View(questions);
    }

    public async Task<IActionResult> CreateManual()
    {
        var questions = await _questionRepository.GetDistinctQuestionsAsync();

        return View(questions);
    }

    [HttpPost]
    public async Task<IActionResult> CreateManual(List<int>? selectedQuestionIds, string name, string description)
    {
        if (selectedQuestionIds?.Count == 0)
        {
            TempData["ErrorMessage"] = "Please select at least one question.";

            return RedirectToAction(nameof(CreateManual));
        }

        var selectedQuestions = await _questionRepository.GetQuestionsWithoutTestIdAsync(selectedQuestionIds);

        var test = new Test
        {
            Name = name,
            Description = description,
            Questions = selectedQuestions,
            NumberOfQuestions = selectedQuestions.Count,
            NumberOfAnswersPerQuestion = selectedQuestions.Max(q => q.Answers.Count)
        };

        await _testRepository.AddTestAsync(test);

        TempData["SuccessMessage"] = "Test created successfully.";

        return RedirectToAction(nameof(Index), nameof(Test));
    }

    [HttpGet]
    public async Task<IActionResult> CreateAuto()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateAuto(string name, string description, int numberOfQuestions)
    {
        var randomQuestions = await _questionRepository.GetQuestionsWithoutTestIdAsync(numberOfQuestions);

        var test = new Test
        {
            Name = name,
            Description = description,
            Questions = randomQuestions,
            NumberOfQuestions = randomQuestions.Count,
            NumberOfAnswersPerQuestion = randomQuestions.Max(q => q.Answers.Count)
        };

        await _testRepository.AddTestAsync(test);

        return RedirectToAction(nameof(Index), nameof(Test));
    }

    [HttpGet]
    public async Task<IActionResult> Add()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(Question question, int numberOfAnswers)
    {
        if (!ModelState.IsValid)
        {
            return View(question);
        }

        if (numberOfAnswers < 2)
        {
            TempData["ErrorMessage"] = "The number of answers per question should be higher than 1.";
      
            return View(question);
        }

        ViewBag.NumberOfAnswers = numberOfAnswers;

        return View(nameof(AddQuestion), question);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddQuestion(Question question)
    {
        if (!ModelState.IsValid)
        {
            return View(question);
        }

        if (!question.Answers.Any(a => a.IsCorrect))
        {
            TempData["ErrorMessage"] = "Please select at least one correct answer.";

            ViewBag.NumberOfAnswers = question.Answers.Count;

            return View(nameof(AddQuestion), question);
        }

        await _questionRepository.AddQuestionAsync(question);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var question = await _questionRepository.GetQuestionAsync(id);

        return View(question);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Question question)
    {
        if (!ModelState.IsValid)
        {
            return View(question);
        }

        var existingQuestion = await _questionRepository.GetQuestionAsync(question.QuestionId);

        existingQuestion.QuestionText = question.QuestionText;
        existingQuestion.Answers = question.Answers;

        await _questionRepository.UpdateQuestionAsync(existingQuestion);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var question = await _questionRepository.GetQuestionAsync(id);

        return View(question);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var question = await _questionRepository.GetQuestionAsync(id);

        await _questionRepository.DeleteQuestionAsync(question.QuestionId);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var question = await _questionRepository.GetQuestionAsync(id);

        return View(question);
    }
}