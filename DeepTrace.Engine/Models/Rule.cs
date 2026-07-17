namespace DeepTrace.Engine.Models;

public class Rule
{
    public string EvidenceId {get; set;} = "";

    public EvidenceValue ExpectedValue { get; set; }

    public double Weight { get; set; }

    public string Reason { get; set; } = "";
}