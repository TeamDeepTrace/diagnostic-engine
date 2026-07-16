namespace DeepTrace.Engine.Logic;

public class ConfidenceCalculator
{
    public Dictionary<string, double> Normalize(Dictionary<string, double> scores)
    {
        double total = scores.Values.Sum();

        Dictionary<string, double> result = [];

        foreach(var item in scores)
        {
            if (total <= 0)
            {
                result[item.Key] = 0;
            } else
            {
                result[item.Key] = Math.Round((item.Value / total) * 100, 2);
            }
        }

        return result;
    }
}