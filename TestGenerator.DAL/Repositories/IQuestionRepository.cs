using TestGenerator.DAL.Models;

namespace TestGenerator.DAL.Repositories;

public interface IQuestionRepository
{
    Task<Question> AddQuestionAsync(Question question);

    Task<Question> GetQuestionAsync(int? id, string currentUserId);

    Task<List<Question>> GetQuestionsAsync(string currentUserId);

    IQueryable<QuestionTestViewModel> GetQuestions(string? sortOrder, string? searchString, string currentUserId);

    Task<List<Question>> GetQuestionsWithoutTestIdAsync(List<int> questionIds, string currentUserId);

    Task<List<Question>> GetQuestionsWithoutTestIdAsync(int numberOfQuestions, string currentUserId);

    Task<bool> UpdateQuestionAsync(Question question, string currentUserId);

    Task<bool> DeleteQuestionAsync(int id, string currentUserId);

    Task<int?> GetNumberOfQuestionsInTheDatabase(string currentUserId);
}