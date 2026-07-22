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
        Assert.Contains("Duplicate problem ID", result.Errors[0]);
    }

    [Fact]
    public void Validate_DuplicateQuestionIds_ReturnsError()
    {
        var pack = new DiagnosticPack
        {
            Name = "Duplicate Question Pack",
            Problems = [],
            Questions = [
                new DiagnosticQuestion {
                    Id = "external_monitor",
                    Text = "Does an external monitor work?"
                },
                new DiagnosticQuestion {
                    Id = "external_monitor",
                    Text = "Is the device still visible on another display?"
                }
            ]
        };

        var validator = new DiagnosticPackValidator();

        var result = validator.Validate(pack);

        Assert.False(result.IsValid);
        Assert.Contains("Duplicate question ID", result.Errors[0]);
    }

    [Fact]
    public void Validate_ValidPack_ReturnsSuccess()
    {
        var pack = new DiagnosticPack
        {
            Name = "Valid Pack",
            Problems = [
                new Problem {
                    Id = "gpu",
                    Name = "GPU Failure",
                    Rules = [
                        new Rule {
                            EvidenceId = "artifacts"
                        }
                    ]
                }
            ],
            Questions = [
                new DiagnosticQuestion {
                    Id = "artifacts_question",
                    Text = "Are there visual artifacts",
                    EvidenceId = "artifacts"
                }
            ]
        };

        var validator = new DiagnosticPackValidator();

        var result = validator.Validate(pack);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}