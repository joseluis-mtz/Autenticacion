using System.ComponentModel.DataAnnotations;

namespace LoginsAU.Models
{
    public class AccesoViewModel
    {
        [Required(ErrorMessage = "Email obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name ="Contraseña")]
        public string Password { get; set; }

        // Para recordar las credenciales del usuario
        [Display(Name = "Recordarme")]
        public bool RememberMe { get; set; }
    }
}
