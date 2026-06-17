using System.ComponentModel.DataAnnotations;

namespace Analisis_Numerico_2026.Models.Unit4
{
    public class IntegrationInput
    {
        [Required(ErrorMessage = "La función a evaluar es requerida.")]
        public string Function { get; set; } = string.Empty;

        [Required(ErrorMessage = "El extremo inferior (Xi) es requerido.")]
        public double Xi { get; set; }

        [Required(ErrorMessage = "El extremo superior (Xd) es requerido.")]
        public double Xd { get; set; }

        [Range(1, 100000, ErrorMessage = "La cantidad de subintervalos debe ser de al menos 1.")]
        public int Subintervalos { get; set; } = 1;

        [Required(ErrorMessage = "El método de integración es requerido.")]
        public string Method { get; set; } = "TrapeciosSimple";
    }
}
