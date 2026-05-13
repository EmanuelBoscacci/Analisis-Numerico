namespace Analisis_Numerico_2026.Models.Unit2
{
    /// <summary>
    /// Datos de entrada para sistemas de ecuaciones lineales.
    /// 
    /// Representa el sistema  A * x = b  donde:
    ///   A = matriz de coeficientes (dimension x dimension)
    ///   b = vector de términos independientes (dimension elementos)
    /// 
    /// En la UI el usuario ingresa la matriz AUMENTADA [A|b],
    /// es decir cada fila tiene (dimension + 1) valores.
    /// </summary>
    public class LinearSystemInput
    {// Número de ecuaciones (y de incógnitas)
        public int Dimension { get; set; } = 3;

        // Método elegido: "GaussJordan" o "GaussSeidel"
        public string Method { get; set; } = string.Empty;

        // Matriz aumentada [A|b] aplanada en una lista para el binding del formulario.
        // Tamaño esperado: dimension * (dimension + 1)
        // Ejemplo dim=3: [ a00,a01,a02,b0, a10,a11,a12,b1, a20,a21,a22,b2 ]
        public List<double> MatrizAumentada { get; set; } = new();

        // ── Solo Gauss-Seidel ──
        public double Tolerancia { get; set; } = 0.0001;
        public int MaxIteraciones { get; set; } = 100;
    }
}
