using System.ComponentModel.DataAnnotations;

namespace LoginsAU.Models
{
    public class ResetPassViewModel
    {
        [Required(ErrorMessage = "Email obligatorio")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
