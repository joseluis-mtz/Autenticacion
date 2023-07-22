using LoginsAU.Models;
using Microsoft.AspNetCore.Mvc;

namespace LoginsAU.Controllers
{
    public class CuentasController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Registro()
        {
            RegistroViewModel regVM = new RegistroViewModel();
            return View(regVM);
        }
    }
}
