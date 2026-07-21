using DeepTrace.Engine;
using DeepTrace.Engine.Loading;
using DeepTrace.Engine.Models;

DiagnosticPackManager manager = new("DiagnosticPacks");
DiagnosticPack pack = manager.Load("SP7");

var engine = new Engine(pack);

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