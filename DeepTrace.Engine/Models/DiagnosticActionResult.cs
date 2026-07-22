namespace DeepTrace.Engine.Models;

public class DiagnosticActionResult
{
    public bool Success { get; set; }

    public DiagnosticError Error { get; set; } = DiagnosticError.None;

    public string? ErrorMessage { get; set; }
}