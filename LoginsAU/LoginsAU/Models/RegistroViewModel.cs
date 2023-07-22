using System.ComponentModel.DataAnnotations;

namespace LoginsAU.Models
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "Email obligatorio")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password obligatorio")]
        [StringLength(50, ErrorMessage = "El {0} debe ser al menos de {2} caracteres de longitud", MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name ="Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmar contraseña por favor")]
        [Compare("Password", ErrorMessage = "Las contraseñan no son iguales")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Nombre obligatorio")]
        public string Nombre { get; set; }
        public string Url { get; set; }
        public int CodigoPais { get; set; }
        public string Telefono { get; set; }

        [Required(ErrorMessage = "Pais obligatorio")]
        public string Pais { get; set; }
        public string Ciudad { get; set; }
        public string Direccion { get; set; }

        [Required(ErrorMessage = "Fecha Nacimiento obligatorio")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "Activo obligatorio")]
        public bool Activo { get; set; }
    }
}
