using System.ComponentModel.DataAnnotations;

namespace TestGenerator.DAL.Models;

public class Test
{
    [Key]
    public int TestId { get; set; }

    [Required]
    public string? Name { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public List<Question>? Questions { get; set; }

    [Required]
    public int NumberOfQuestions { get; set; }

    [Required]
    public int NumberOfAnswersPerQuestion { get; set; }
}