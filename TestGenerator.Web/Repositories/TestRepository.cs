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

    public async Task<Test> FindAsync(int? id)
    {
        return await _dbContext.Tests.FindAsync(id);
    }

    public async Task<Test> AddTest(Test test)
    {
        _dbContext.Tests?.Add(test);
        await _dbContext.SaveChangesAsync();
        return test;
    }

    public async Task<List<Test>> GetTests()
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

    public bool TestExists(int id)
    {
        return _dbContext.Tests.Any(e => e.TestId == id);
    }
}