using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplicationAnalisisNumerico.Models;
using WebApplicationAnalisisNumerico.Services;

namespace WebApplicationAnalisisNumerico.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly RootFinder _rootFinder;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _rootFinder = new RootFinder();
        }

        public IActionResult Index()
        {
            return View(new RootFindingInput());
        }

        [HttpPost]
        public IActionResult Index(RootFindingInput input)
        {
            if (!ModelState.IsValid)
            {
                return View(input);
            }

            var result = _rootFinder.FindRoot(input);
            ViewData["Result"] = result;
            return View(input);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
