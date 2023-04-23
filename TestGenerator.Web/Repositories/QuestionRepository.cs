using Microsoft.EntityFrameworkCore;
using TestGenerator.DAL.Data;
using TestGenerator.DAL.Models;

namespace TestGenerator.Web.Repositories;

public class QuestionRepository : IQuestionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public QuestionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Question> AddQuestionAsync(Question question)
    {
        _dbContext.Questions?.Add(question);

        await _dbContext.SaveChangesAsync();

        return question;
    }

    public async Task<Question> GetQuestionAsync(int? id)
    {
        return await _dbContext.Questions
            .Include(question => question.Answers)
            .FirstOrDefaultAsync(question => question.QuestionId == id);
    }

    public async Task<List<Question>> GetDistinctQuestionsAsync()
    {
        return await _dbContext.Questions.Distinct().ToListAsync();
    }

    public async Task<List<Question>> GetQuestionsByIdsWithoutTestIdAsync(List<int> questionIds)
    {
        var questionsWithoutTestId = await _dbContext.Questions
            .Include(question => question.Answers)
            .Where(q => questionIds.Contains(q.QuestionId))
            .Where(q => q.TestId == null)
            .ToListAsync();

        var duplicatedQuestionsWithoutTestId = await GetDuplicatedQuestionsWithoutTestIdFromQuestionsWithTestIdAsync(questionIds);

        questionsWithoutTestId.AddRange(duplicatedQuestionsWithoutTestId);

        return questionsWithoutTestId;
    }

    public async Task<Question> UpdateQuestionAsync(Question question)
    {
        _dbContext.Entry(question).State = EntityState.Modified;

        await _dbContext.SaveChangesAsync();

        return question;
    }

    public async Task<bool> DeleteQuestionAsync(int id)
    {
        var question = await _dbContext.Questions.FindAsync(id);

        if (question == null)
        {
            return false;
        }

        var answers = question.Answers.ToList(); // Get the answers associated with the question

        foreach (var answer in answers)
        {
            _dbContext.Answers.Remove(answer); // Remove each answer from the context
        }

        _dbContext.Questions.Remove(question);

        await _dbContext.SaveChangesAsync();

        return true;
    }

    private async Task<List<Question>> GetDuplicatedQuestionsWithoutTestIdFromQuestionsWithTestIdAsync(List<int> questionIds)
    {
        var questionsToDuplicate = await _dbContext.Questions
            .Include(q => q.Answers)
            .Where(q => questionIds.Contains(q.QuestionId))
            .Where(q => q.TestId != null)
            .ToListAsync();

        var duplicatedQuestions = new List<Question>();

        foreach (var question in questionsToDuplicate)
        {
            var duplicatedQuestion = new Question
            {
                QuestionText = question.QuestionText,
                TestId = null, // set to null so it's not associated with the original test
                Answers = new List<Answer>()
            };

            foreach (var answer in question.Answers)
            {
                var duplicatedAnswer = new Answer
                {
                    AnswerText = answer.AnswerText,
                    IsCorrect = answer.IsCorrect
                };

                duplicatedQuestion.Answers.Add(duplicatedAnswer);
            }

            duplicatedQuestions.Add(duplicatedQuestion);
        }

        return duplicatedQuestions;
    }
}