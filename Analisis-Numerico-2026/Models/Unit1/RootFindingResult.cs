namespace Analisis_Numerico_2026.Models.Unit1
{
    public class RootFindingResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public double Root { get; set; }
        public int IterationsUsed { get; set; }
        public double FinalError { get; set; }
        public List<RootIterationResult> Iterations { get; set; } = new();
        public string Method { get; set; } = string.Empty;
        public string Function { get; set; } = string.Empty;
    }
}
