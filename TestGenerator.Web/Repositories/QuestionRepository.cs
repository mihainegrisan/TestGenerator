﻿using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Question>> GetQuestionsAsync()
    {
        return await _dbContext.Questions.ToListAsync();
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
}