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

    private readonly DiagnosticPack _pack;

    public Engine(DiagnosticPack pack)
    {
        _problems = pack.Problems;
        _questions = pack.Questions;
        _settings = pack.Settings;
        _pack = pack;
    }

    public DiagnosticSession CreateSession(string device)
    {
        return new DiagnosticSession
        {
            Device = device,
            State = DiagnosticState.Running
        };
    }

    public DiagnosticActionResult SubmitAnswer(DiagnosticSession session, string questionId, EvidenceValue anwser)
    {
        var questionExists = _pack.Questions.Any(q => q.Id == questionId);

        if (!questionExists)
        {
            return new DiagnosticActionResult
            {
                Success = false,
                Error = DiagnosticError.QuestionNotFound,
                ErrorMessage = $"Question '{questionId}' was not found."
            };
        }

        session.AskedQuestions.Add(questionId);

        session.Evidence.Add(
            new Evidence
            {
                Id = questionId,
                Value = anwser
            }
        );

        return new DiagnosticActionResult
        {
            Success = true,
            Error = DiagnosticError.None
        };
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

        if (session.State != DiagnosticState.Running)
        {
            return null;
        }

        DiagnosticQuestion? nextQuestion = _selector.SelectNextQuestion(
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
