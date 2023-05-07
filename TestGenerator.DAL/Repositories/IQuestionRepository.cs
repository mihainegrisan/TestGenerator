using TestGenerator.DAL.Models;

namespace TestGenerator.DAL.Repositories;

public interface IQuestionRepository
{
    Task<Question> AddQuestionAsync(Question question);

    Task<Question> GetQuestionAsync(int? id);

    Task<List<Question>> GetQuestionsAsync();

    IQueryable<Question> GetQuestions(string? sortOrder, string? searchString);

    Task<List<Question>> GetQuestionsWithoutTestIdAsync(List<int> questionIds);

    Task<List<Question>> GetQuestionsWithoutTestIdAsync(int numberOfQuestions);

    Task<Question> UpdateQuestionAsync(Question question);

    Task<bool> DeleteQuestionAsync(int id);
}