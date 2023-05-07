using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TestGenerator.DAL.Models;

public class Question
{
    public int? TestId { get; set; }

    [Key] public int QuestionId { get; set; }

    [Required]
    [StringLength(1000, ErrorMessage = "{0} length can't be more than {1}.")]
    public string? QuestionText { get; set; }

    public List<Answer>? Answers { get; set; }

    //public bool IsCreatedManually { get; set; }
    //public bool IsAutoCreatedFromQuestions { get; set; }
    //public bool IsAutoCreatedByChatGpt { get; set; }

    public DateTime? CreatedAt { get; set; }

    public IdentityUser? Author { get; set; }
    public string? AuthorId { get; set; }
}