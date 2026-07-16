namespace DeepTrace.Engine.Models;

public class Reason
{
    public string EvidenceId { get; set; } = "";

    public string Description { get; set; } = "";

    public double Weight { get; set; }

    public EvidenceValue ExpectedValue { get; set; }
    
    public EvidenceValue ActualValue { get; set; }
}