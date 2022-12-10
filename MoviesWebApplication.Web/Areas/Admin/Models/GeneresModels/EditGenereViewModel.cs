using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Admin.Models.GeneresModels
{
    public class EditGenereViewModel:CreateGenereViewModel
    {
        [Required]
        public int Id { get; set; }
        public string ReturnUrl { get; set; }
    }
}
