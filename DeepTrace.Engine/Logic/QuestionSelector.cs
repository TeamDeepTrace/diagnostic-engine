using DeepTrace.Engine.Models;

namespace DeepTrace.Engine.Logic;

public class QuestionSelector
{

    public DiagnosticQuestion? SelectNextQuestion(DiagnosticSession session, List<DiagnosticQuestion> questions, List<DiagnosticResult> results, List<Problem> problems, EngineSettings settings)
    {
        List<QuestionPriority> questionPriorities = [];

        var rankedResults = results
            .OrderByDescending(r => r.Score)
            .ToList();

        if (rankedResults.Count == 0)
            return null;

        var bestScore = rankedResults[0].Score;

        var candidateResults = rankedResults
            .Where(r => r.Score >= bestScore * settings.ProblemThreshold)
            .ToList();

        foreach (var result in candidateResults)
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

                if (question == null)
                    continue;
                
                var priority = CalculateQuestionPriority(question, candidateResults);

                var alreadyAdded = false;

                foreach (QuestionPriority questionPriority in questionPriorities)
                {
                    if (questionPriority.Question.Id == question.Id)
                    {
                        alreadyAdded = true;
                        break;
                    }
                }

                if (!alreadyAdded)
                    questionPriorities.Add(priority);
            }
        }

        if (questionPriorities.Count < 1)
            return null;

        // PrintQuestionPriorities(questionPriorities);

        return questionPriorities.OrderByDescending(r => r.Priority).ToList().First().Question;
    }

    public QuestionPriority CalculateQuestionPriority(DiagnosticQuestion question, List<DiagnosticResult> results)
    {
        double score = 0;

        foreach (DiagnosticResult result in results)
        {
            List<Rule> rules = result.Problem.Rules;

            foreach (Rule rule in rules)
            {
                if (rule.EvidenceId == question.EvidenceId)
                {
                    score += (result.Confidence / 100) * rule.Weight;
                }
            }
        }

        return new QuestionPriority
        {
            Question = question,
            Priority = score
        };
    }

    // public void PrintQuestionPriorities(List<QuestionPriority> questionPriorities)
    // {
    //     Console.WriteLine();

    //     Console.WriteLine("Question Ranking:");
    //     foreach(QuestionPriority questionPriority in questionPriorities)
    //     {
    //         Console.WriteLine();
    //         Console.WriteLine($"{questionPriority.Question.Text}");
    //         Console.WriteLine($"Priority: {questionPriority.Priority}");
    //     }
    // }
}