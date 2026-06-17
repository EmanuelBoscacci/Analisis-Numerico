using Analisis_Numerico_2026.Models.Unit3;
using Analisis_Numerico_2026.Services.Unit3;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Analisis_Numerico_2026.Pages.Unit3
{
    public class IndexModel : PageModel
    {
        private readonly RegressionService _regressionService;

        public IndexModel(RegressionService regressionService)
        {
            _regressionService = regressionService;
        }

        [BindProperty]
        public RegressionInput Input { get; set; } = new();

        public RegressionResult? Result { get; set; }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                return;
            }

            Result = _regressionService.CalcularRegresion(Input);
        }
    }
}
