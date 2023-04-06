namespace TestGenerator.DAL.Models;

public class TestItem
{
    public int TestItemId { get; set; }
    public string? Question { get; set; }
    public IEnumerable<string>? Answers { get; set; }
    public int CorrectAnswerIndex { get; set; }
}