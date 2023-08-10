﻿using LoginsAU.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace LoginsAU.Controllers
{
    public class CuentasController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IEmailSender _emailSender;
        public CuentasController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Registro(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            RegistroViewModel regVM = new RegistroViewModel();
            return View(regVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
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

        private void ValidarErrores(IdentityResult Resultado)
        {
            foreach (var item in Resultado.Errors) 
            {
                ModelState.AddModelError(String.Empty, item.Description);
            }
        }

        [HttpGet]
        public IActionResult Acceso(string returnUrl = null)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public IActionResult ResetPass() 
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        public IActionResult CambiarPass(string code = null)
        {
            return code == null ? View("Error") : View();
        }
    }

}
