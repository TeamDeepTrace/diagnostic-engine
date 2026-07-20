namespace DeepTrace.Engine.Models;

public class DiagnosticPack
{
    public string Name { get; set; } = "";

    public List<Problem> Problems { get; set; } = [];

    public List<DiagnosticQuestion> Questions { get; set; } = [];

    public EngineSettings Settings { get; set; } = new EngineSettings();
}