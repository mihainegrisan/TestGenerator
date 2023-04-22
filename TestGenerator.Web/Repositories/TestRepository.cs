using Microsoft.EntityFrameworkCore;
using TestGenerator.DAL.Data;
using TestGenerator.DAL.Models;

namespace TestGenerator.Web.Repositories;

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
        return await _dbContext.Tests.ToListAsync();
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