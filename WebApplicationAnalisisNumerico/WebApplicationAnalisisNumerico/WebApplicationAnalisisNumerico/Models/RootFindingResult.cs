using System.Collections.Generic;

namespace WebApplicationAnalisisNumerico.Models
{
    public class RootFindingResult
    {
        public bool Converged { get; set; }
        public double Root { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<IterationResult> Iterations { get; set; } = new List<IterationResult>();
    }
}