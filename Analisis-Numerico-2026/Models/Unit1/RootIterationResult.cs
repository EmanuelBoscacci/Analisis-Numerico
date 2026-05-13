namespace Analisis_Numerico_2026.Models.Unit1
{
    public class RootIterationResult
    {
        public int Iteration { get; set; }
        public double Xi { get; set; }
        public double Xd { get; set; }
        public double Xr { get; set; }
        public double FunctionValue { get; set; }
        public double RelativeError { get; set; }
        public bool IsRoot { get; set; }
    }
}
