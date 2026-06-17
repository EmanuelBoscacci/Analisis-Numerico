using System.Collections.Generic;

namespace Analisis_Numerico_2026.Models.Unit3
{
    public class RegressionResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<double[]> Puntos { get; set; } = new();

        // Regresión Lineal
        public bool LinearSuccess { get; set; }
        public string LinearMessage { get; set; } = string.Empty;
        public string LinearFuncionObtenida { get; set; } = string.Empty;
        public double LinearR { get; set; }
        public double LinearPearsonR { get; set; }
        public bool LinearAjusteAceptable { get; set; }
        public List<double> LinearCoeficientes { get; set; } = new();

        // Regresión Polinomial
        public bool PolySuccess { get; set; }
        public string PolyMessage { get; set; } = string.Empty;
        public string PolyFuncionObtenida { get; set; } = string.Empty;
        public double PolyR { get; set; }
        public double PolyPearsonR { get; set; }
        public bool PolyAjusteAceptable { get; set; }
        public List<double> PolyCoeficientes { get; set; } = new();
        public int PolyGrado { get; set; }
    }
}
