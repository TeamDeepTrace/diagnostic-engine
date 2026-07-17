using DeepTrace.Engine;
using DeepTrace.Engine.Models;

var displayCable = new Problem
{
    Id = "display_cable",
    Name = "Display Cable Failure",

    Rules = [
        new Rule {
            EvidenceId = "external_monitor",
            ExpectedValue = EvidenceValue.Yes,
            Weight = 40,
            Reason = "External monitor works, indicating the GPU is likely functioning."
        },

        new Rule {
            EvidenceId = "vertical_lines",
            ExpectedValue = EvidenceValue.Yes,
            Weight = 30,
            Reason = "Vertical lines are a common symptom of a damaged display cable."
        },

        new Rule {
            EvidenceId = "touch_works",
            ExpectedValue = EvidenceValue.Yes,
            Weight = 15,
            Reason = "Touch still works, suggesting only the display path is affected."
        }
    ]
};

var lcd = new Problem
{
    Id = "lcd",
    Name = "LCD Failure",

    Rules = [
        new Rule {
            EvidenceId = "vertical_lines",
            ExpectedValue = EvidenceValue.Yes,
            Weight = 25,
            Reason = "Vertical lines can indicate an LCD panel failure."
        }
    ]
};

var gpu = new Problem
{
    Id = "gpu",
    Name = "GPU Failure",

    Rules =
    [
        new Rule
        {
            EvidenceId = "external_monitor",
            ExpectedValue = EvidenceValue.No,
            Weight = 50,
            Reason = "External display failure can indicate a GPU output issue."
        },

        new Rule
        {
            EvidenceId = "artifacts",
            ExpectedValue = EvidenceValue.Yes,
            Weight = 40,
            Reason = "Visual artifacts are common with GPU failures."
        }
    ]
};

var questions = new List<DiagnosticQuestion>
{
    new()
    {
        Id = "external_monitor",
        Text = "Does an external monitor work?",
        EvidenceId = "external_monitor",
        Description = "Connect the device to an external monitor or TV using a compatible video cable or adapter. If the external display shows a normal image, select Yes."
    },

    new()
    {
        Id = "vertical_lines",
        Text = "Are there vertical lines on the display?",
        EvidenceId = "vertical_lines",
        Description = "Look closely at the built-in display. Select Yes if you see one or more vertical lines that remain visible regardless of what is displayed on the screen."
    },

    new()
    {
        Id = "touch_works",
        Text = "Does touch still work?",
        EvidenceId = "touch_works",
        Description = "Try tapping and dragging on different parts of the touchscreen. Select Yes if touch input responds normally even if the display image appears distorted."
    },

    new()
    {
        Id = "artifacts",
        Text = "Does the output look wrong in any way?",
        EvidenceId = "artifacts",
        Description = "Select Yes if any parts of the display appear distorted."
    }
};

var engine = new Engine(
    [displayCable, lcd, gpu], 
    questions,
    new EngineSettings
    {
        ProblemThreshold = 0.5
    });

var session = engine.CreateSession("Surface Pro 7");

while (!engine.IsComplete(session))
{
    var question =
        engine.GetNextQuestion(session);

    if (question == null)
        break;

    Console.WriteLine(question.Text);

    Console.Write("(y/n): ");

    var input =
        Console.ReadLine();

    var answer =
        input?.ToLower() == "y"
            ? EvidenceValue.Yes
            : EvidenceValue.No;

    engine.SubmitAnswer(
        session,
        question.Id,
        answer);

    Console.WriteLine();
    Console.WriteLine("Current Rankings:");

    foreach (var result in engine.GetResults(session))
    {
        Console.WriteLine(
            $"  {result.Problem.Name}: {result.Score} ({result.Confidence:F1}%)");
    }

    Console.WriteLine();
}

var results =
    engine.GetResults(session);

foreach(var result in results)
{
    Console.WriteLine();

    Console.WriteLine(
        $"Problem: {result.Problem.Name}");

    Console.WriteLine(
        $"Evidence Score: {result.Score}");

    Console.WriteLine(
        $"Confidence: {result.Confidence:F2}%");

    Console.WriteLine("Reasons:");

    foreach(var reason in result.Reasons)
    {
        Console.WriteLine(
            $"  • {reason.Description} ({reason.Weight:+0;-0})");
    }
}