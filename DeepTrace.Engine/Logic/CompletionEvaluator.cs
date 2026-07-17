using DeepTrace.Engine.Models;

namespace DeepTrace.Engine.Logic;

public class CompletionEvaluator
{
    public DiagnosticState Evaluate(DiagnosticSession session, List<DiagnosticResult> results, EngineSettings settings)
    {
        if (results.Count < 1)
            return DiagnosticState.NoDiagnosis;
        
        if (session.Evidence.Count < settings.MinimumQuestions)
            return DiagnosticState.Running;
        
        if (results.OrderByDescending(r => r.Confidence).First().Confidence >= settings.CompletionThreshold)
            return DiagnosticState.Completed;

        return DiagnosticState.Running;
    }
}