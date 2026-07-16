namespace DeepTrace.Engine.Models;

public class Problem
{
    public string Id { get; set; } = "";

    public string Name { get; set; } = "";

    public List<Rule> Rules { get; set; } = [];
}