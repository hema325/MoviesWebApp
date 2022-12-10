using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Admin.Models.GeneresModels
{
    public class CreateGenereViewModel
    {
        [Required]
        public string Name { get; set; }
    }
}
