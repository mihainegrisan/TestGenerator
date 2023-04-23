using System.ComponentModel.DataAnnotations;

namespace TestGenerator.DAL.Models;

public class Question
{
    public int? TestId { get; set; }

    [Key] public int QuestionId { get; set; }

    [Required] public string? QuestionText { get; set; }

    public List<Answer>? Answers { get; set; }
}