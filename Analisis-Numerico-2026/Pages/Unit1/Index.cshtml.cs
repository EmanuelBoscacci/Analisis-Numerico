using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Analisis_Numerico_2026.Models.Unit1;
using Analisis_Numerico_2026.Services.Unit1;

namespace Analisis_Numerico_2026.Pages.Unit1
{
    public class IndexModel : PageModel
    {
        private readonly RootFindingService _rootService;

        public IndexModel(RootFindingService rootService)
        {
            _rootService = rootService;
        }

        [BindProperty]
        public RootFindingInput Input { get; set; } = new();

        public RootFindingResult? Result { get; set; }

        public void OnGet()
        {
        }

        public void OnPost()
        {
            if (!ModelState.IsValid)
                return;

            Result = Input.Method switch
            {
                "Biseccion" => _rootService.Biseccion(Input),
                "ReglaFalsa" => _rootService.ReglaFalsa(Input),
                "NewtonRaphson" => _rootService.NewtonRaphson(Input),
                "Secante" => _rootService.Secante(Input),
                _ => _rootService.Biseccion(Input)
            };
        }
    }
}