namespace DeepTrace.Engine.Models;

public class DiagnosticQuestion
{
    public string Id { get; set; } = "";

    public string Text { get; set; } = "";

    public string EvidenceId { get; set; } = "";

    public string? Description { get; set; }

    public double Importance { get; set; }
}