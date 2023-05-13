using Microsoft.EntityFrameworkCore;
using TestGenerator.DAL.Data;
using TestGenerator.DAL.Models;

namespace TestGenerator.DAL.Repositories;

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

    public async Task<Question> GetQuestionAsync(int? id, string currentUserId)
    {
        return await _dbContext.Questions
            .Include(question => question.Answers)
            .Where(q => q.AuthorId == currentUserId)
            .AsNoTracking()
            .FirstOrDefaultAsync(question => question.QuestionId == id);
    }

    public async Task<int?> GetNumberOfQuestionsInTheDatabase(string currentUserId)
    {
        return await _dbContext.Questions
            .Where(q => q.AuthorId == currentUserId)
            .AsNoTracking()
            .CountAsync();
    }

    public IQueryable<QuestionTestViewModel> GetQuestions(string? sortOrder, string? searchString, string currentUserId)
    {
        var questions = _dbContext.Questions
            .Where(q => q.AuthorId == currentUserId)
            .AsNoTracking()
            .Select(q => q);

        var questionViewModels = questions.Select(question => new QuestionTestViewModel
        {
            Question = question,
            Test = _dbContext.Tests.AsNoTracking().FirstOrDefault(test => test.TestId == question.TestId)
        });

        questionViewModels = sortOrder switch
        {
            "question_name_desc" => questionViewModels.OrderByDescending(q => q.Question.QuestionText),
            "test_name_desc" => questionViewModels.OrderByDescending(t => t.Test.Name),
            "test_name" => questionViewModels.OrderBy(t => t.Test.Name),
            "date" => questionViewModels.OrderBy(t => t.Question.CreatedAt),
            "date_desc" => questionViewModels.OrderByDescending(q => q.Question.CreatedAt),
            _ => questionViewModels.OrderBy(q => q.Question.QuestionText)
        };

        if (!string.IsNullOrEmpty(searchString))
        {
            questionViewModels = questionViewModels.Where(q => q.Question.QuestionText.Contains(searchString, StringComparison.InvariantCultureIgnoreCase));

            // search by test name
            //if (!questionViewModels.Any())
            //{
            //    questionViewModels = questionViewModels.Where(q => q.Test.Name.Contains(searchString));
            //}
        }

        return questionViewModels;
    }

    public async Task<List<Question>> GetQuestionsAsync(string currentUserId)
    {
        return await _dbContext.Questions
            .Where(q => q.AuthorId == currentUserId)
            .ToListAsync();
    }

    public async Task<List<Question>> GetQuestionsWithoutTestIdAsync(List<int> questionIds, string currentUserId)
    {
        var questionsWithoutTestId = await _dbContext.Questions
            .Include(question => question.Answers)
            .Where(q => q.AuthorId == currentUserId)
            .Where(q => questionIds.Contains(q.QuestionId))
            .Where(q => q.TestId == null)
            .ToListAsync();

        var duplicatedQuestionsWithoutTestId = await GetDuplicatedQuestionsWithoutTestIdFromQuestionsWithTestIdAsync(questionIds, currentUserId);

        questionsWithoutTestId.AddRange(duplicatedQuestionsWithoutTestId);

        return questionsWithoutTestId;
    }

    public async Task<List<Question>> GetQuestionsWithoutTestIdAsync(int numberOfQuestions, string currentUserId)
    {
        var selectedQuestions = await GetRandomQuestions(numberOfQuestions, currentUserId);

        var selectedQuestionsIds = selectedQuestions.Select(q => q.QuestionId).ToList();

        var duplicatedQuestionsWithoutTestId = await GetDuplicatedQuestionsWithoutTestIdFromQuestionsWithTestIdAsync(selectedQuestionsIds, currentUserId);

        var questionsWithoutTestId = selectedQuestions.Where(q => q.TestId == null).ToList();

        questionsWithoutTestId.AddRange(duplicatedQuestionsWithoutTestId);

        return questionsWithoutTestId;
    }


    public async Task<bool> UpdateQuestionAsync(Question updatedQuestion, string currentUserId)
    {
        var oldQuestion = await _dbContext.Questions
            .Include(q => q.Answers)
            .AsNoTracking()
            .FirstOrDefaultAsync(q => q.QuestionId == updatedQuestion.QuestionId && q.AuthorId == currentUserId);

        if (oldQuestion == null)
        {
            return false;
        }

        updatedQuestion.TestId = oldQuestion.TestId;
        updatedQuestion.EditedAt = DateTime.Now;

        try
        {
            _dbContext.Questions.Update(updatedQuestion);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> DeleteQuestionAsync(int id, string currentUserId)
    {
        var question = await _dbContext.Questions
            .Where(q => q.AuthorId == currentUserId)
            .SingleOrDefaultAsync(t => t.TestId == id);

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

    private async Task<List<Question>> GetRandomQuestions(int numberOfQuestions, string currentUserId)
    {
        // Get a list of all questions in the database
        var allQuestions = await _dbContext.Questions
            .Include(question => question.Answers)
            .Where(q => q.AuthorId == currentUserId)
            .ToListAsync();

        var shuffledQuestions = allQuestions.OrderBy(q => Guid.NewGuid()).ToList();

        var selectedQuestions = shuffledQuestions.Take(numberOfQuestions).ToList();

        return selectedQuestions;
    }

    private async Task<List<Question>> GetDuplicatedQuestionsWithoutTestIdFromQuestionsWithTestIdAsync(List<int> questionIds, string currentUserId)
    {
        var questionsToDuplicate = await _dbContext.Questions
            .Include(q => q.Answers)
            .Where(q => questionIds.Contains(q.QuestionId))
            .Where(q => q.TestId != null)
            .Where(q => q.AuthorId == currentUserId)
            .ToListAsync();

        var duplicatedQuestions = new List<Question>();

        foreach (var question in questionsToDuplicate)
        {
            var duplicatedQuestion = new Question
            {
                QuestionText = question.QuestionText,
                TestId = null,
                CreatedAt = DateTime.Now,
                Author = question.Author,
                AuthorId = question.AuthorId,
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