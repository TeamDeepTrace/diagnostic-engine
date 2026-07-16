using DeepTrace.Engine.Models;

namespace DeepTrace.Engine.Logic;

public class ScoringEngine
{
    public double CalculateScore(Problem problem, DiagnosticSession session)
    {
        double score = 0;

        foreach (var rule in problem.Rules)
        {
            var evidence = session.Evidence.FirstOrDefault(
                x => x.Id == rule.EvidenceId
            );

            if(evidence != null &&
            evidence.Value == rule.ExpectedValue)
            {
                score += rule.Weight;
            }
        }

        return score;
    }

    public List<Reason> GetReasons(Problem problem, DiagnosticSession session) {
        List<Reason> reasons = [];

        foreach (var rule in problem.Rules)
        {
            var evidence = session.Evidence.FirstOrDefault(
                e => e.Id == rule.EvidenceId);

            if (evidence != null &&
                evidence.Value == rule.ExpectedValue)
            {
                reasons.Add(new Reason
                {
                    EvidenceId = rule.EvidenceId,
                    Description = rule.Reason,
                    Weight = rule.Weight,
                    ExpectedValue = rule.ExpectedValue,
                    ActualValue = evidence.Value
                });
            }
        }

        return reasons;
    }
}