using System.ComponentModel.DataAnnotations;

namespace TestGenerator.DAL.Models;

public class Answer
{
    [Key]
    public int AnswerId { get; set; }

    [Required]
    public string? AnswerText { get; set; }

    [Required]
    public bool IsCorrect { get; set; }
}
