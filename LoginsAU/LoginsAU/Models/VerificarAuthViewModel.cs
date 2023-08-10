using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace LoginsAU.Models
{
    public class VerificarAuthViewModel
    {
        [Required]
        [Display(Name = "Código del autenticador")]
        public string Code { get; set; }
        public string ReturnURL { get; set; }

        [Display(Name = "Recodar datos")]
        public bool RecordarDatos { get; set; }
    }
}
