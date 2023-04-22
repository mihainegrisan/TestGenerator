﻿using Microsoft.AspNetCore.Mvc;
using TestGenerator.DAL.Models;
using TestGenerator.Web.Repositories;
using TestGenerator.Web.Services;

namespace TestGenerator.Web.Controllers;

public class TestController : Controller
{
    private readonly IFileProcessor _fileProcessor;
    private readonly ITestRepository _testRepository;

    public TestController(ITestRepository testRepository, IFileProcessor fileProcessor)
    {
        _testRepository = testRepository;
        _fileProcessor = fileProcessor;
    }

    public async Task<IActionResult> Index()
    {
        var tests = await _testRepository.GetTestsAsync();
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

        await _testRepository.AddTestAsync(test);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> DownloadPdf(int id)
    {
        var test = await _testRepository.GetTestAsync(id);

        var stream = _fileProcessor.GeneratePdf(test);

        stream.Position = 0;

        return File(stream, "application/pdf", $"{test.Name}.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var test = await _testRepository.GetTestAsync(id);

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

        var existingTest = await _testRepository.GetTestAsync(test.TestId);

        existingTest.Name = test.Name;
        existingTest.Description = test.Description;
        existingTest.NumberOfQuestions = test.NumberOfQuestions;
        existingTest.NumberOfAnswersPerQuestion = test.NumberOfAnswersPerQuestion;
        existingTest.Questions = test.Questions;

        await _testRepository.UpdateTestAsync(existingTest);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var test = await _testRepository.GetTestAsync(id);

        return View(test);
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var test = await _testRepository.GetTestAsync(id);

        await _testRepository.DeleteTestAsync(test.TestId);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var test = await _testRepository.GetTestAsync(id);

        return View(test);
    }
}