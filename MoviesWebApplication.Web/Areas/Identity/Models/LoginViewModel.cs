using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Identity.Models
{
    public class LoginViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(maximumLength:24,MinimumLength =6, ErrorMessage = "Password must be at least 6 charachters and at most 24 charachters")]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }
        public bool IsPersistent { get; set; }
    }
}
