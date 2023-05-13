using Microsoft.EntityFrameworkCore;
using TestGenerator.DAL.Data;
using TestGenerator.DAL.Models;

namespace TestGenerator.DAL.Repositories;

public class TestRepository : ITestRepository
{
    private readonly ApplicationDbContext _dbContext;

    public TestRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Test> AddTestAsync(Test test)
    {
        _dbContext.Tests?.Add(test);

        await _dbContext.SaveChangesAsync();

        return test;
    }

    public async Task<Test> GetTestAsync(int? id)
    {
        return await _dbContext.Tests
            .Include(test => test.Questions)
            .ThenInclude(question => question.Answers)
            .FirstOrDefaultAsync(test => test.TestId == id);
    }

    public async Task<List<Test>> GetTestsAsync()
    {
        return await _dbContext.Tests.AsNoTracking().ToListAsync();
    }

    public IQueryable<Test> GetTests(string? sortOrder, string? searchString, bool isAutoCreatedByChatGpt)
    {
        var tests = isAutoCreatedByChatGpt
            ? _dbContext.Tests.AsNoTracking().Where(t => t.IsAutoCreatedByChatGpt).Select(t => t)
            : _dbContext.Tests.AsNoTracking().Where(t => !t.IsAutoCreatedByChatGpt).Select(t => t);

        if (!string.IsNullOrEmpty(searchString))
        {
            tests = tests.Where(t => t.Name.Contains(searchString));
        }

        tests = sortOrder switch
        {
            "name_desc" => tests.OrderByDescending(t => t.Name),
            "date" => tests = tests.OrderBy(t => t.CreatedAt),
            "date_desc" => tests = tests.OrderByDescending(t => t.CreatedAt),
            _ => tests.OrderBy(t => t.Name)
        };

        return tests.AsNoTracking();
    }

    public async Task<bool> UpdateTestAsync(Test updatedTest)
    {
        var oldTest = await _dbContext.Tests
            .Include(t => t.Questions)
            //.ThenInclude(q => q.Answers)
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.TestId == updatedTest.TestId);

        if (oldTest == null)
        {
            return false;
        }

        updatedTest.EditedAt = DateTime.Now;

        foreach (var question in updatedTest.Questions)
        {
            var oldQuestion = oldTest.Questions.FirstOrDefault(q => q.QuestionId == question.QuestionId);

            if (oldQuestion == null)
            {
                continue;
            }

            question.TestId = oldQuestion.TestId;
            question.AuthorId = oldQuestion.AuthorId;
            question.Author = oldQuestion.Author;
            question.CreatedAt = oldQuestion.CreatedAt;
            question.EditedAt = DateTime.Now;
        }

        try
        {
            _dbContext.Tests.Update(updatedTest);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> DeleteTestAsync(int id)
    {
        var test = await _dbContext.Tests.FindAsync(id);

        if (test == null)
        {
            return false;
        }

        _dbContext.Tests.Remove(test);

        await _dbContext.SaveChangesAsync();

        return true;
    }
}