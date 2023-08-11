using LoginsAU.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Drawing;

namespace LoginsAU.Controllers
{
    [Authorize]
    public class CuentasController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        public CuentasController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _roleManager = roleManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(string returnUrl = null)
        {
            if (!await _roleManager.RoleExistsAsync("Administrador"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Administrador"));
            }

            if (!await _roleManager.RoleExistsAsync("Registrado"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Registrado"));
            }

            ViewData["returnUrl"] = returnUrl;
            RegistroViewModel regVM = new RegistroViewModel();
            return View(regVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Registro(RegistroViewModel Registro, string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
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
                    //
                    await _userManager.AddToRoleAsync(Usuario, "Administrador");
                    // Confirmacion de MAIL
                    var codigo = await _userManager.GenerateEmailConfirmationTokenAsync(Usuario);
                    var urlRetorno = Url.Action("ConfirmarEmail", "Cuentas", new { userId = Usuario.Id, code = codigo }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(Registro.Email, "Confirmar cuenta - LoginsAU", "Confirme su cuenta dando click aqui: <a href = \"" + urlRetorno + ">Recuperar mi contraseña</a>");

                    await _signInManager.SignInAsync(Usuario, isPersistent: false);
                    //return RedirectToAction("Index", "Home");
                    return LocalRedirect(returnUrl);
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

        [AllowAnonymous]
        private void ValidarErrores(IdentityResult Resultado)
        {
            foreach (var item in Resultado.Errors) 
            {
                ModelState.AddModelError(String.Empty, item.Description);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Acceso(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Acceso(AccesoViewModel Acceso, string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                var resultado = await _signInManager.PasswordSignInAsync(Acceso.Email, Acceso.Password, Acceso.RememberMe, lockoutOnFailure : true);
                if (resultado.Succeeded)
                {
                    //return RedirectToAction("Index", "Home");
                    return LocalRedirect(returnUrl);
                }
                else if (resultado.IsLockedOut)
                {
                    //return RedirectToAction("Index", "Home");
                    return View("Bloqueado");
                }
                else if (resultado.RequiresTwoFactor) 
                {
                    return RedirectToAction("VerificarCodAuth", new { returnUrl, Acceso.RememberMe });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CerrarSesion()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPass() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPass(ResetPassViewModel opViewModel)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByEmailAsync(opViewModel.Email);
                if (usuario == null)
                {
                    return RedirectToAction("ConfirmarPassword");
                }
                else
                {
                    var codigo = await _userManager.GeneratePasswordResetTokenAsync(usuario);
                    var urlRetorno = Url.Action("CambiarPass", "Cuentas", new { userId = usuario.Id, code = codigo }, protocol: HttpContext.Request.Scheme);
                    await _emailSender.SendEmailAsync(opViewModel.Email, "Recuperar contraseña - LoginsAU", "Recupere su contraseña dando click aqui: <a href = \"" + urlRetorno + ">Recuperar mi contraseña</a>");
                    return RedirectToAction("ConfirmarPassword");
                }
            }
            else
            {
                return View(opViewModel);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmarPassword()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult CambiarPass(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> CambiarPass(RecuperaPassViewModel rpViewModel)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.FindByEmailAsync(rpViewModel.Email);
                if (usuario == null)
                {
                    return RedirectToAction("ConfirmaRecuperacionPassword");
                }
                else
                {
                    var resultado = await _userManager.ResetPasswordAsync(usuario, rpViewModel.Code, rpViewModel.Password);
                    if (resultado.Succeeded)
                    {
                        return RedirectToAction("ConfirmaRecuperacionPassword");
                    }
                    else
                    {
                        ValidarErrores(resultado);
                        return View();
                    }
                    
                }
            }
            else
            {
                return View(rpViewModel);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ConfirmaRecuperacionPassword()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmarEmail(string userId, string codigo)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(codigo))
            {
                return View("Error");
            }
            else
            {
                var usuario = await _userManager.FindByIdAsync(userId);
                if (usuario == null)
                {
                    return View("Error");
                }
                else
                {
                    var resultado = await _userManager.ConfirmEmailAsync(usuario, codigo);
                    if (resultado.Succeeded)
                    {
                        return View("ConfirmarEmail");
                    }
                    else
                    {
                        return View("Error");
                    }
                }
            }

        }

        [HttpGet]
        public async Task<IActionResult> ActivarAutenticador()
        {
            var usuario = await _userManager.GetUserAsync(User);
            await _userManager.ResetAuthenticatorKeyAsync(usuario);
            var token = await _userManager.GetAuthenticatorKeyAsync(usuario);
            var authDosFactores = new DosFactoresViewModel() { Token = token };
            return View(authDosFactores);
        }

        [HttpPost]
        public async Task<IActionResult> ActivarAutenticador(DosFactoresViewModel dosFactoresVM)
        {
            if (ModelState.IsValid)
            {
                var usuario = await _userManager.GetUserAsync(User);
                var exito = await _userManager.VerifyTwoFactorTokenAsync(usuario, _userManager.Options.Tokens.AuthenticatorTokenProvider, dosFactoresVM.Code);
                if (exito)
                {
                    await _userManager.SetTwoFactorEnabledAsync(usuario, true);
                }
                else
                {
                    ModelState.AddModelError("Error", "Autenticación de dos factores no validada");
                    return View(dosFactoresVM);
                }
            }
            return RedirectToAction(nameof(ConfirmacionAuth));
        }
        [HttpGet]
        public async Task<IActionResult> DesactivarAutenticador()
        {
            var usuario = await _userManager.GetUserAsync(User);
            await _userManager.ResetAuthenticatorKeyAsync(usuario);
            await _userManager.SetTwoFactorEnabledAsync(usuario, false);
            return RedirectToAction(nameof(Index), "Home");
        }

        [HttpGet]
        public IActionResult ConfirmacionAuth()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerificarCodAuth(bool recordarDatos, string returnUrl = null)
        {
            var usuario = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (usuario == null)
            {
                return View("Error");
            }
            else
            {
                ViewData["returnUrl"] = returnUrl;
                return View(new VerificarAuthViewModel { ReturnURL = returnUrl, RecordarDatos = recordarDatos});
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> VerificarCodAuth(VerificarAuthViewModel veauVM)
        {
            veauVM.ReturnURL = veauVM.ReturnURL ?? Url.Content("~/");
            if (!ModelState.IsValid)
            {
                return View(veauVM);
            }
            else
            {
                var resultado = await _signInManager.TwoFactorAuthenticatorSignInAsync(veauVM.Code, veauVM.RecordarDatos, rememberClient: true);
                if (resultado.Succeeded)
                {
                    return LocalRedirect(veauVM.ReturnURL);
                }
                else if (resultado.IsLockedOut)
                {
                    return View("Bloqueado");
                }
                else 
                {
                    ModelState.AddModelError(string.Empty, "Código inválido");
                    return View();
                }
            }
        }
    }

}
