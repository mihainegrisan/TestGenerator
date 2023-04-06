using System.ComponentModel.DataAnnotations;

namespace TestGenerator.DAL.Models;

public class Test
{
    [Key]
    public int TestId { get; set; }

    [Required]
    public List<Question>? Questions { get; set; }
}