using DeepTrace.Engine.Models;
using DeepTrace.Engine.Logic;

namespace DeepTrace.Engine;

public class Engine
{
    private readonly List<Problem> _problems;

    private readonly List<DiagnosticQuestion> _questions;

    private readonly EngineSettings _settings;

    private readonly ScoringEngine _scoring = new();

    private readonly ConfidenceCalculator _confidence = new();

    private readonly QuestionSelector _selector = new();

    private readonly CompletionEvaluator _completion = new();

    public Engine(List<Problem> problems, List<DiagnosticQuestion> questions, EngineSettings settings)
    {
        _problems = problems;
        _questions = questions;
        _settings = settings;
    }

    public DiagnosticSession CreateSession(string device)
    {
        return new DiagnosticSession
        {
            Device = device,
            State = DiagnosticState.Running
        };
    }

    public void SubmitAnswer(DiagnosticSession session, string questionId, EvidenceValue anwser)
    {
        session.AskedQuestions.Add(questionId);

        session.Evidence.Add(
            new Evidence
            {
                Id = questionId,
                Value = anwser
            }
        );
    }

    public List<DiagnosticResult> GetResults(DiagnosticSession session)
    {
        Dictionary<string, double> scores = [];

        foreach (var problem in _problems)
        {
            scores[problem.Name] = _scoring.CalculateScore(problem, session);
        }

        var confidence = _confidence.Normalize(scores);

        List<DiagnosticResult> results = [];

        foreach (var problem in _problems)
        {
            results.Add(
                new DiagnosticResult
                {
                    Problem = problem,
                    Score = scores[problem.Name],
                    Confidence = confidence[problem.Name],
                    Reasons = _scoring.GetReasons(problem, session)
                }
            );
        }

        return results.OrderByDescending(r => r.Score).ToList();
    }

    public DiagnosticQuestion? GetNextQuestion(DiagnosticSession session)
    {
        var results = GetResults(session);

        session.State = _completion.Evaluate(session, results, _settings);

        Console.WriteLine($"State: {session.State}");

        if (session.State != DiagnosticState.Running)
        {
            return null;
        }

        DiagnosticQuestion nextQuestion = _selector.SelectNextQuestion(
            session,
            _questions,
            results,
            _problems,
            _settings);

        return nextQuestion;
    }

    public bool IsComplete(DiagnosticSession session)
    {
        return GetNextQuestion(session) == null;
    }
}
