using System.ComponentModel.DataAnnotations;

namespace TestGenerator.DAL.Models;

public class Tag
{
  [Key]
  public int Id { get; set; }

  public string? Name { get; set; }

  public List<QuestionTag>? QuestionTags { get; set; }
}
