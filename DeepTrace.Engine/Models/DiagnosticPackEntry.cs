namespace DeepTrace.Engine.Models;

public class DiagnosticPackEntry
{
    public string Name { get; set; } = "";

    public List<string> Aliases { get; set; } = [];

    public string File { get; set; } = "";
}