namespace DeepTrace.Engine.Models;

public class DiagnosticPackValidatorResult
{
    public bool IsValid { get; set; }

    public List<string> Errors { get; set; } = [];
}