namespace Analisis_Numerico_2026.Models.Unit2
{
    /// <summary>
    /// Resultado de resolver un sistema de ecuaciones lineales.
    /// </summary>
    public class LinearSystemResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public int Dimension { get; set; }

        // Vector solución: x1, x2, ..., xn
        public List<double> Solucion { get; set; } = new();

        // Gauss-Jordan: pasos de eliminación
        public List<EliminationStep> Pasos { get; set; } = new();

        // Gauss-Seidel: historial de iteraciones
        public List<SeidelIterationStep> IteracionesSeidel { get; set; } = new();
    }
}
