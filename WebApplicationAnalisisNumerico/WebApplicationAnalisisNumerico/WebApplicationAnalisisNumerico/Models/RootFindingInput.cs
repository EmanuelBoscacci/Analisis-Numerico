namespace WebApplicationAnalisisNumerico.Models
{
    public class RootFindingInput
    {
        public string Function { get; set; } = string.Empty;
        public string Method { get; set; } = "Secante"; // "Secante" o "Tangente"
        public double Xi { get; set; }
        public double Xd { get; set; }
        public double Tolerance { get; set; } = 1e-6;
        public int MaxIterations { get; set; } = 50;
    }
}