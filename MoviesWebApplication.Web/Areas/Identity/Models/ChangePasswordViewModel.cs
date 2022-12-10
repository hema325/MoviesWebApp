using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Identity.Models
{

    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }

    public class ConfirmChangePasswordViewModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[\w!@#$%^&*()_+]{6,24}$", ErrorMessage = "Password must be at least 6 charachters and at most 24 charachters")]
        public string Password { get; set; }
        [Display(Name = "Confirm Password")]
        [Compare(nameof(Password))]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

}
