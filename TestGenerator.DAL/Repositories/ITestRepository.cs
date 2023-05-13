using TestGenerator.DAL.Models;

namespace TestGenerator.DAL.Repositories;

public interface ITestRepository
{
    Task<Test> AddTestAsync(Test test);

    Task<Test> GetTestAsync(int? id);

    Task<List<Test>> GetTestsAsync();

    IQueryable<Test> GetTests(string? sortOrder, string? searchString, bool isAutoCreatedByChatGpt);

    Task<bool> UpdateTestAsync(Test updatedTest);

    Task<bool> DeleteTestAsync(int id, bool? deleteQuestions);
}