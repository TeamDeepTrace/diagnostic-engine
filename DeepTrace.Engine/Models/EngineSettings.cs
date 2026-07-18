namespace DeepTrace.Engine.Models;

public class EngineSettings
{
    public double ProblemThreshold { get; set; } = 0.5;

    public double CompletionThreshold { get; set; } = 90;

    public int MinimumQuestions { get; set; } = 3;

    public double ConfidenceGapThreshold { get; set; } = 20;
}