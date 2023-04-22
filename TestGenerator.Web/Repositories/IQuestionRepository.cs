using TestGenerator.DAL.Models;

namespace TestGenerator.Web.Repositories;

public interface IQuestionRepository
{
    Task<Question> AddQuestionAsync(Question question);

    Task<Question> GetQuestionAsync(int? id);

    Task<List<Question>> GetQuestionsAsync();

    Task<Question> UpdateQuestionAsync(Question question);

    Task<bool> DeleteQuestionAsync(int id);
}