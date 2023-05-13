using TestGenerator.DAL.Models;

namespace TestGenerator.DAL.Repositories;

public interface ITestRepository
{
    Task<Test> AddTestAsync(Test test);

    Task<Test> GetTestAsync(int? id, string currentUserId);

    Task<List<Test>> GetTestsAsync(string currentUserId);

    IQueryable<Test> GetTests(string? sortOrder, string? searchString, bool isAutoCreatedByChatGpt, string currentUserId);

    Task<bool> UpdateTestAsync(Test updatedTest, string currentUserId);

    Task<bool> DeleteTestAsync(int id, bool? deleteQuestions, string currentUserId);
}