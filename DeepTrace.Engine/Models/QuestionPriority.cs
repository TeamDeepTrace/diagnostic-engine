namespace DeepTrace.Engine.Models;

public class QuestionPriority
{
    public required DiagnosticQuestion Question { get; set; }

    public double Priority { get; set; }
}