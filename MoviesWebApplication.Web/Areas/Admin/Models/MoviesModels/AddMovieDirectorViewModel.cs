using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Admin.Models.MoviesModels
{
    public class AddMovieDirectorViewModel
    {
        [Required]
        public int MovieId { get; set; }
        [Required]
        public int DirectorId { get; set; }
        public IEnumerable<SelectListItem> DirectorsList { get; set; }
        public string returnUrl { get; set; }
    }
}
