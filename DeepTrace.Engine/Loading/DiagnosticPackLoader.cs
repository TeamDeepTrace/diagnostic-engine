using DeepTrace.Engine.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeepTrace.Engine.Loading;

public class DiagnosticPackLoader
{
    public DiagnosticPack Load(string path)
    {
        var options = new JsonSerializerOptions
        {
            Converters =
                {
                    new JsonStringEnumConverter()
                }
        };

        if (!File.Exists(path))
        {
            throw new FileNotFoundException(
                "Diagnostic pack file not found.",
                path);
        }

        string json = File.ReadAllText(path);
        DiagnosticPack? pack = JsonSerializer.Deserialize<DiagnosticPack>(json, options);

        if (pack == null)
            throw new InvalidDataException($"InvalidDataException {path}");

        return pack;
    }
}