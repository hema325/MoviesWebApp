using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Identity.Models
{
    public class ChangeImageViewModel
    {
        [Required]
        [Display(Name ="Choose Your Image")]
        [DataType(DataType.ImageUrl)]
        public IFormFile Image { get; set; }
    }
}
