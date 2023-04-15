using System.ComponentModel.DataAnnotations;

namespace TestGenerator.DAL.Models;

public class Question
{
    [Key]
    public int QuestionId { get; set; }

    [Required]
    public string? QuestionText { get; set; }

    [Required]
    public List<Answer>? Answers { get; set; }
}
