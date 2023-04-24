using System.ComponentModel.DataAnnotations;

namespace TestGenerator.DAL.Models;

public class Test
{
    [Key] public int TestId { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "{0} length can't be more than {1}.")]
    public string? Name { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "{0} length can't be more than {1}.")]
    public string? Description { get; set; }

    public List<Question>? Questions { get; set; }

    [Required] [Range(1, 50)] public int NumberOfQuestions { get; set; }

    [Required] [Range(2, 10)] public int NumberOfAnswersPerQuestion { get; set; }

    public bool IsCreatedManually { get; set; }
    public bool IsAutoCreatedFromQuestions { get; set; }
    public bool IsAutoCreatedByChatGpt { get; set; }
}