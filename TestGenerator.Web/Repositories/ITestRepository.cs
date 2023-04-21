using TestGenerator.DAL.Models;

namespace TestGenerator.Web.Repositories;

public interface ITestRepository
{
    Task<Test> FindAsync(int? id);
    Task<Test> AddTest(Test test);
    Task<Test> GetTest(int id);
    Task<List<Test>> GetTests();
    Task<Test> UpdateTestAsync(Test test);
    Task<bool> DeleteTestAsync(int id);
    bool TestExists(int id);
}