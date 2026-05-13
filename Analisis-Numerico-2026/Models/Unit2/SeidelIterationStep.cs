namespace Analisis_Numerico_2026.Models.Unit2
{
    /// <summary>
    /// Representa el estado del vector solución en cada iteración de Gauss-Seidel.
    /// </summary>
    public class SeidelIterationStep
    {
        // Número de iteración
        public int Iteracion { get; set; }

        // Valores de x1..xn en esta iteración
        public List<double> Vector { get; set; } = new();

        // Error relativo por componente (vacío en la primera iteración)
        public List<double> Errores { get; set; } = new();
    }
}
