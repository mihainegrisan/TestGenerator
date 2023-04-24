using System.ComponentModel.DataAnnotations;

namespace TestGenerator.DAL.Models;

public class Question
{
    public int? TestId { get; set; }

    [Key] public int QuestionId { get; set; }

    [Required]
    [StringLength(1000, ErrorMessage = "{0} length can't be more than {1}.")]
    public string? QuestionText { get; set; }

    public List<Answer>? Answers { get; set; }
}