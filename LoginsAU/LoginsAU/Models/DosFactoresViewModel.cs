using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace LoginsAU.Models
{
    public class DosFactoresViewModel
    {
        [Required]
        [Display(Name = "Código del autenticador")]
        public string Code { get; set; }
        public string Token { get; set; }
    }
}
