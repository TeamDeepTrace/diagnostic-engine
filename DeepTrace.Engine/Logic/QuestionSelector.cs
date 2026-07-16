using DeepTrace.Engine.Models;

namespace DeepTrace.Engine.Logic;

public class QuestionSelector
{
    public DiagnosticQuestion? SelectNextQuestion(DiagnosticSession session, List<DiagnosticQuestion> questions, List<DiagnosticResult> results, List<Problem> problems)
    {
        foreach (var result in results.OrderByDescending(r => r.Score))
        {
            var problem = result.Problem;

            foreach (var rule in problem.Rules)
            {
                bool alreadyKnown =
                    session.Evidence.Any(
                        e => e.Id == rule.EvidenceId);

                if (alreadyKnown)
                    continue;

                var question =
                    questions.FirstOrDefault(
                        q => q.EvidenceId == rule.EvidenceId);

                if (question != null)
                    return question;
            }
        }

        return null;
    }
}