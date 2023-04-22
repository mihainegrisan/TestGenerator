using TestGenerator.DAL.Models;

namespace TestGenerator.Web.Repositories;

public interface ITestRepository
{
    Task<Test> AddTestAsync(Test test);

    Task<Test> GetTestAsync(int? id);

    Task<List<Test>> GetTestsAsync();

    Task<Test> UpdateTestAsync(Test test);

    Task<bool> DeleteTestAsync(int id);
}