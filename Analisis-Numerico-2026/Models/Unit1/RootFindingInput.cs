namespace Analisis_Numerico_2026.Models.Unit1
{
    public class RootFindingInput
    {
        public string Function { get; set; } = string.Empty;
        public double Xi { get; set; }
        public double Xd { get; set; }
        public double Tolerance { get; set; }
        public int MaxIterations { get; set; }
        public string Method { get; set; } = string.Empty;
    }
}
