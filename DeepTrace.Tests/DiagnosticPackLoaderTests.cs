using DeepTrace.Engine.Loading;
using Xunit;

namespace DeepTrace.Tests;

public class DiagnosticPackLoaderTests
{
    [Fact]
    public void Load_ValidDiagnosticPack_ReturnsPack()
    {
        var loader = new DiagnosticPackLoader();

        string path = Path.Combine("DiagnosticPacks", "SurfacePro7.json");

        var pack = loader.Load(path);

        Assert.NotNull(pack);
        Assert.Equal("Surface Pro 7", pack.Name);
        Assert.NotNull(pack.Problems);
        Assert.NotNull(pack.Questions);
    }
}