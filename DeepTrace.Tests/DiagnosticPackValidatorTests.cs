using System.Runtime.Intrinsics.Wasm;
using DeepTrace.Engine.Models;
using DeepTrace.Engine.Validation;
using Xunit;

namespace DeepTrace.Tests;

public class DiagnosticPackValidatorTests
{
    [Fact]
    public void Validate_ProblemWithoutId_ReturnsError()
    {
        var pack = new DiagnosticPack
        {
            Name = "Broken Pack",
            Problems = [
                new Problem {
                    Name = "Broken Display",
                    Id = ""
                }
            ],
            Questions = []
        };

        var validator = new DiagnosticPackValidator();

        var result = validator.Validate(pack);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("missing an ID", result.Errors[0]);
    }

    [Fact]
    public void Validate_RuleReferencesMissingEvidence_ReturnsError()
    {
        var pack = new DiagnosticPack
        {
            Name = "Broken Evidence Pack",
            Problems = [
                new Problem {
                    Id = "gpu",
                    Name = "GPU Failure",
                    Rules = [
                        new Rule {
                            EvidenceId = "fake_evidence"
                        }
                    ]
                }
            ],
            Questions = []
        };

        var validator = new DiagnosticPackValidator();

        var result = validator.Validate(pack);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("missing evidence", result.Errors[0]);
    }

    [Fact]
    public void Validate_DuplicateProblemIds_ReturnsError()
    {
        var pack = new DiagnosticPack
        {
            Name = "Duplicate ID Pack",
            Problems = [
                new Problem {
                    Id = "gpu",
                    Name = "GPU Failure"
                },
                new Problem {
                    Id = "gpu",
                    Name = "GPU Failure"
                }
            ],
            Questions = []
        };

        var validator = new DiagnosticPackValidator();

        var result = validator.Validate(pack);

        Assert.False(result.IsValid);
    }
}