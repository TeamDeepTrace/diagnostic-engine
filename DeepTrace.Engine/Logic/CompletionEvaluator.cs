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

        List<DiagnosticResult> sortedResults = results.OrderByDescending(r => r.Confidence).ToList();

        DiagnosticResult firstResult = sortedResults[0];

        if (firstResult.Confidence < settings.CompletionThreshold)
            return DiagnosticState.Running;
        
        if (sortedResults.Count > 1)
        {
            DiagnosticResult secondResult = sortedResults[1];
            if (firstResult.Confidence - secondResult.Confidence <= settings.ConfidenceGapThreshold)
                return DiagnosticState.Running;
        }

        return DiagnosticState.Completed;
    }
}