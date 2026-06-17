using Analisis_Numerico_2026.Models.Unit4;
using Analisis_Numerico_2026.Services.Unit4;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Analisis_Numerico_2026.Pages.Unit4
{
    public class IndexModel : PageModel
    {
        private readonly IntegrationService _integrationService;

        public IndexModel(IntegrationService integrationService)
        {
            _integrationService = integrationService;
        }

        [BindProperty]
        public IntegrationInput Input { get; set; } = new()
        {
            Xi = 0.5,
            Xd = 3.5,
            Subintervalos = 4
        };

        public IntegrationResult? Result { get; set; }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                return;
            }

            Result = _integrationService.Integrar(Input);
        }
    }
}
