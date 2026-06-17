using System.ComponentModel.DataAnnotations;

namespace Analisis_Numerico_2026.Models.Unit3
{
    public class RegressionInput
    {
        public string Method { get; set; } = "RegresionLineal";

        [Required(ErrorMessage = "La tolerancia es requerida.")]
        [Range(0.0001, 100.0, ErrorMessage = "La tolerancia debe estar entre 0.0001 y 100.")]
        public double Tolerancia { get; set; } = 80.0; // Valor por defecto 80%

        [Required(ErrorMessage = "Debe ingresar al menos dos puntos para el cálculo.")]
        public string RawPoints { get; set; } = string.Empty;

        [Range(1, 10, ErrorMessage = "El grado del polinomio debe estar entre 1 y 10.")]
        public int Grado { get; set; } = 2;
    }
}
