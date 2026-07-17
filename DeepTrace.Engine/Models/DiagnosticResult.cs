namespace DeepTrace.Engine.Models;

public class DiagnosticResult
{
    public Problem Problem { get; set; } = null!;

    public double Score { get; set; }

    public double Confidence { get; set; }

    public List<Reason> Reasons { get; set; } = [];
}