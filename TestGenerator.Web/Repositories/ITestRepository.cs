using TestGenerator.DAL.Models;

namespace TestGenerator.Web.Repositories;

public interface ITestRepository
{
    Test Find(int? id);
    Task<Test> AddTest(Test test);
    Task<Test> GetTest(int id);
    Task<List<Test>> GetTests();
    Task<Test> UpdateTest(Test test);
    Task<bool> DeleteTestAsync(int id);
}