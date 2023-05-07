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
            _ => tests.OrderBy(t => t.Name)
        };

        return tests.AsNoTracking();
    }

    public async Task<Test> UpdateTestAsync(Test test)
    {
        _dbContext.Entry(test).State = EntityState.Modified;

        await _dbContext.SaveChangesAsync();

        return test;
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