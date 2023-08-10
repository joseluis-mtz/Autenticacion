using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace LoginsAU.Models
{
    public class RecuperaPassViewModel
    {
        [Required(ErrorMessage = "Email obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password obligatorio")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmar contraseña por favor")]
        [Compare("Password", ErrorMessage = "Las contraseñan no son iguales")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; }
        public string Code { get; set; }
    }
}
