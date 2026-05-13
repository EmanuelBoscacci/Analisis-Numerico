using Analisis_Numerico_2026.Models.Unit2;
using Analisis_Numerico_2026.Services.Unit2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Analisis_Numerico_2026.Pages.Unit2
{
    public class IndexModel : PageModel
    {
        private readonly GaussJordanService _gaussJordanService;
        private readonly GaussSeidelService _gaussSeidelService;

        public IndexModel(GaussJordanService gaussJordanService, GaussSeidelService gaussSeidelService)
        {
            _gaussJordanService = gaussJordanService;
            _gaussSeidelService = gaussSeidelService;
        }

        [BindProperty]
        public LinearSystemInput Input { get; set; } = new();

        public LinearSystemResult? Result { get; set; }

        public void OnGet() { }

        public void OnPost()
        {
            if (!ModelState.IsValid)
                return;

            Result = Input.Method switch
            {
                "GaussJordan" => _gaussJordanService.Resolver(Input),
                "GaussSeidel" => _gaussSeidelService.Resolver(Input),
                _ => new LinearSystemResult
                {
                    Success = false,
                    Message = $"Método '{Input.Method}' no reconocido",
                    Method = Input.Method ?? string.Empty,
                    Dimension = Input.Dimension,
                    Solucion = new List<double>(),
                    Pasos = new List<EliminationStep>(),
                    IteracionesSeidel = new List<SeidelIterationStep>()
                }
            };
        }
    }
}
