using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Identity.Models
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(maximumLength:24,MinimumLength =6,ErrorMessage ="Password must be at least 6 charachters and at most 24 charachters")]
        public string Password { get; set; }
        [Display(Name ="Confirm Password")]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
