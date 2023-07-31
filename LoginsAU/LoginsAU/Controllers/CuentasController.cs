using LoginsAU.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LoginsAU.Controllers
{
    public class CuentasController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public CuentasController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
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
        [HttpPost]
        public async Task<IActionResult> Registro(RegistroViewModel Registro)
        {
            if (ModelState.IsValid)
            {
                var Usuario = new AppUsuario
                {
                    UserName = Registro.Email,
                    Email = Registro.Email,
                    Nombre = Registro.Nombre,
                    Url = Registro.Url,
                    CodigoPais = Registro.CodigoPais,
                    Telefono = Registro.Telefono,
                    Pais = Registro.Pais,
                    Ciudad = Registro.Ciudad,
                    Direccion = Registro.Direccion,
                    FechaNacimiento = Registro.FechaNacimiento,
                    Activo = Registro.Activo,
                };
                var resultado = await _userManager.CreateAsync(Usuario, Registro.Password);
                if (resultado.Succeeded)
                {
                    await _signInManager.SignInAsync(Usuario, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ValidarErrores(resultado);
                    return View(Registro);
                }
            }
            else 
            {
                return View(Registro);
            }
        }

        private void ValidarErrores(IdentityResult Resultado)
        {
            foreach (var item in Resultado.Errors) 
            {
                ModelState.AddModelError(String.Empty, item.Description);
            }
        }

        [HttpGet]
        public IActionResult Acceso()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Acceso(AccesoViewModel Acceso)
        {
            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync(Acceso.Email, Acceso.Password, Acceso.RememberMe, lockoutOnFailure : false);
                if (resultado.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Acceso no válido");
                    return View(Acceso);
                }
            }
            else
            {
                return View(Acceso);
            }
        }
    }

}
