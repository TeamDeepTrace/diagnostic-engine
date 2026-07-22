using DeepTrace.Engine.Models;
using DeepTrace.Engine.Validation;
using System.Text.Json;

namespace DeepTrace.Engine.Loading;

public class DiagnosticPackManager
{
    private readonly Dictionary<string, string> _packPaths = new(StringComparer.OrdinalIgnoreCase);
    
    private readonly string _packDirectory;

    private readonly DiagnosticPackLoader _loader = new();

    private readonly DiagnosticPackValidator _validator = new();

    public DiagnosticPackManager(string packDirectory)
    {
        string indexPath = Path.Combine(packDirectory, "Index.json");

        if (!File.Exists(indexPath))
        {
            throw new FileNotFoundException(
                "Diagnostic Index file not found.",
                indexPath);
        }

        string json = File.ReadAllText(indexPath);
        List<DiagnosticPackEntry>? diagnosticEntries = JsonSerializer.Deserialize<List<DiagnosticPackEntry>>(json);

        if (diagnosticEntries == null)
            throw new InvalidDataException($"InvalidDataException {indexPath}");
        
        foreach (DiagnosticPackEntry pack in diagnosticEntries)
        {
            _packPaths[pack.Name] = pack.File;

            foreach (string alias in pack.Aliases)
            {
                _packPaths[alias] = pack.File;
            }
        }

        _packDirectory = packDirectory;
    }

    public DiagnosticPackLoaderResult Load(string name)
    {
        if (!_packPaths.TryGetValue(name, out var file))
            throw new KeyNotFoundException($"Diagnostic pack '{name}' was not found in the index.");
        
        string path = Path.Combine(_packDirectory, file);

        var pack = _loader.Load(path);

        var validation = _validator.Validate(pack);

        if (!validation.IsValid)
        {
            return new DiagnosticPackLoaderResult
            {
                Success = false,
                Errors = validation.Errors
            };
        }

        return new DiagnosticPackLoaderResult
        {
            Success = true,
            Pack = pack
        };
    }
}