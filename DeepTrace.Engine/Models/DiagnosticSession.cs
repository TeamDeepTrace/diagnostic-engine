    namespace DeepTrace.Engine.Models;

    public class DiagnosticSession
    {
        public Guid Id { get; } = Guid.NewGuid();

        public string Device { get; set; } = "";

        public List<Evidence> Evidence { get; } = [];

        public HashSet<string> AskedQuestions { get; } = [];

        public bool Complete { get; set; }

        public DiagnosticState State { get; set; }
    }