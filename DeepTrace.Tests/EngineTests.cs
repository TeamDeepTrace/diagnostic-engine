using System.ComponentModel.DataAnnotations;
using System.IO.Pipelines;
using DeepTrace.Engine;
using DeepTrace.Engine.Loading;
using DeepTrace.Engine.Models;
using Xunit;

namespace DeepTrace.Tests;

public class EngineTests
{
    [Fact]
    public void DisplayCableFailure_WhenSymptomsMatch_ReturnsDisplayCableFailure()
    {
        var manager = new DiagnosticPackManager("DiagnosticPacks");

        var loadResult = manager.Load("Surface Pro 7");

        Assert.True(loadResult.Success);

        var engine = new Engine.Engine(loadResult.Pack!);

        var session = engine.CreateSession("Surface Pro 7");

        engine.SubmitAnswer(session, "external_monitor", Engine.Models.EvidenceValue.Yes);
        engine.SubmitAnswer(session, "vertical_lines", Engine.Models.EvidenceValue.Yes);
        engine.SubmitAnswer(session, "touch_works", Engine.Models.EvidenceValue.Yes);

        var results = engine.GetResults(session);

        var topResult = results.OrderByDescending(r => r.Confidence).First();

        Assert.Equal("display_cable", topResult.Problem.Id);
    }


    [Fact]
    public void GPUFailure_WhenSymptomsMatch_ReturnsGPUFailure()
    {
        var manager = new DiagnosticPackManager("DiagnosticPacks");

        var loadResult = manager.Load("Surface Pro 7");

        Assert.True(loadResult.Success);

        var engine = new Engine.Engine(loadResult.Pack!);

        var session = engine.CreateSession("Surface Pro 7");

        engine.SubmitAnswer(session, "external_monitor", Engine.Models.EvidenceValue.No);
        engine.SubmitAnswer(session, "artifacts", Engine.Models.EvidenceValue.Yes);

        var results = engine.GetResults(session);

        var topResult = results.OrderByDescending(r => r.Confidence).First();

        Assert.Equal("gpu", topResult.Problem.Id);
    }

    [Fact]
    public void SelectNextQuestion_NoEvidence_ReturnsHighestPriorityQuestion()
    {
        var manager = new DiagnosticPackManager("DiagnosticPacks");

        var loadResult = manager.Load("Surface Pro 7");

        Assert.True(loadResult.Success);

        var engine = new Engine.Engine(loadResult.Pack!);

        var session = engine.CreateSession("Surface Pro 7");

        var question = engine.GetNextQuestion(session);

        Assert.NotNull(question);
        Assert.Equal("external_monitor", question.Id);
    }

    [Fact]
    public void SelectNextQuestion_AfterExternalMonitorAnswer_ReturnsNextBestQuestion()
    {
        var manager = new DiagnosticPackManager("DiagnosticPacks");

        var loadResult = manager.Load("Surface Pro 7");

        Assert.True(loadResult.Success);

        var engine = new Engine.Engine(loadResult.Pack!);

        var session = engine.CreateSession("Surface Pro 7");

        engine.SubmitAnswer(session, "external_monitor", Engine.Models.EvidenceValue.Yes);

        var question = engine.GetNextQuestion(session);

        Assert.NotNull(question);
        Assert.Equal("vertical_lines", question.Id);
    }

    [Fact]
    public void IsComplete_NotEnoughEvidence_ReturnsFalse()
    {
        var manager = new DiagnosticPackManager("DiagnosticPacks");

        var loadResult = manager.Load("Surface Pro 7");

        Assert.True(loadResult.Success);

        var engine = new Engine.Engine(loadResult.Pack!);

        var session = engine.CreateSession("Surface Pro 7");

        bool complete = engine.IsComplete(session);

        Assert.False(complete);
    }

    [Fact]
    public void IsComplete_HighConfidenceDiagnosis_ReturnsTrue()
    {
        var manager = new DiagnosticPackManager("DiagnosticPacks");

        var loadResult = manager.Load("Surface Pro 7");

        Assert.True(loadResult.Success);

        var engine = new Engine.Engine(loadResult.Pack!);

        var session = engine.CreateSession("Surface Pro 7");

        engine.SubmitAnswer(session, "external_monitor", Engine.Models.EvidenceValue.Yes);
        engine.SubmitAnswer(session, "vertical_lines", Engine.Models.EvidenceValue.Yes);
        engine.SubmitAnswer(session, "touch_works", Engine.Models.EvidenceValue.Yes);
        engine.SubmitAnswer(session, "artifacts", Engine.Models.EvidenceValue.No);

        bool complete = engine.IsComplete(session);

        Assert.True(complete);
    }

    [Fact]
    public void NoUsefulEvidence_ReturnsNoDiagnosis()
    {
        var pack = new Engine.Models.DiagnosticPack
        {
            Name = "Empty Pack",
            Problems = [],
            Questions = [],
            Settings = new Engine.Models.EngineSettings()
        };

        var engine = new Engine.Engine(pack);

        var session = engine.CreateSession("Empty Pack");

        engine.GetNextQuestion(session);

        Assert.Equal(DiagnosticState.NoDiagnosis, session.State);
    }

    [Fact]
    public void SubmitAnswer_InvalidQuestion_ReturnsError()
    {
        var manager = new DiagnosticPackManager("DiagnosticPacks");

        var loadResult = manager.Load("Surface Pro 7");

        Assert.True(loadResult.Success);

        var engine = new Engine.Engine(loadResult.Pack!);

        var session = engine.CreateSession("Surface Pro 7");

        var result = engine.SubmitAnswer(session, "this_question_does_not_exist", Engine.Models.EvidenceValue.Yes);

        Assert.False(result.Success);
        Assert.Equal(DiagnosticError.QuestionNotFound, result.Error);
        Assert.NotNull(result.ErrorMessage);
    }
}