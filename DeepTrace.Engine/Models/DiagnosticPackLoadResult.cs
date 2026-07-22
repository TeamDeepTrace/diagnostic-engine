namespace DeepTrace.Engine.Models;

public class DiagnosticPackLoaderResult
{
    public bool Success { get; set; }

    public DiagnosticPack? Pack { get; set; }

    public List<string> Errors { get; set; } = [];
}