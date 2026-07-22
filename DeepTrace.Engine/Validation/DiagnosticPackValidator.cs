using System.Security.Cryptography.X509Certificates;
using DeepTrace.Engine.Models;

namespace DeepTrace.Engine.Validation;

public class DiagnosticPackValidator
{
    public DiagnosticPackValidatorResult Validate(DiagnosticPack pack)
    {
        var result = new DiagnosticPackValidatorResult();

        foreach (Problem problem in pack.Problems)
        {
            if (string.IsNullOrWhiteSpace(problem.Id))
            {
                result.Errors.Add($"Problem '{problem.Name}' is missing an ID,");
            }
        }

        var evidenceIds = pack.Questions.Select(q => q.EvidenceId).ToHashSet();

        foreach (Problem problem in pack.Problems)
        {
            foreach (Rule rule in problem.Rules)
            {
                if (!evidenceIds.Contains(rule.EvidenceId))
                {
                    result.Errors.Add($"Problem '{problem.Id}' references missing evidence '{rule.EvidenceId}'.");
                }
            }
        }

        var duplicateProblemIds = pack.Problems.GroupBy(p => p.Id).Where(g => g.Count() > 1);

        foreach (var group in duplicateProblemIds)
        {
            result.Errors.Add($"Duplicate problem ID '{group.Key}'.");
        }

        var duplicateQuestionIds = pack.Questions.GroupBy(q => q.Id).Where(g => g.Count() > 1);

        foreach (var group in duplicateQuestionIds)
        {
            result.Errors.Add($"Duplicate question ID '{group.Key}'.");
        }

        result.IsValid = result.Errors.Count == 0;

        return result;
    }
}