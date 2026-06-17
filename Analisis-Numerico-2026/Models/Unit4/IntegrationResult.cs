namespace Analisis_Numerico_2026.Models.Unit4
{
    public class IntegrationResult
    {
        public string Method { get; set; } = string.Empty;
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public double Area { get; set; }
        public string Function { get; set; } = string.Empty;
    }
}
