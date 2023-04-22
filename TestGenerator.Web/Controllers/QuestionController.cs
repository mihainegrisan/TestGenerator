using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using TestGenerator.DAL.Models;
using TestGenerator.Web.Repositories;
using TestGenerator.Web.Services;

namespace TestGenerator.Web.Controllers;

public class QuestionController : Controller
{
    private readonly IQuestionRepository _questionRepository;

    public QuestionController(IQuestionRepository questionRepository, IFileProcessor fileProcessor)
    {
        _questionRepository = questionRepository;
    }

    public async Task<IActionResult> Index()
    {
        var questions = await _questionRepository.GetQuestionsAsync();

        return View(questions);
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