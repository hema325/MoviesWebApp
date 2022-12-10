using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Admin.Models.MoviesModels
{
    public class CreateMovieViewModel
    {
        [Required]
        public string Title { get; set; }
        [Required]
        [Display(Name ="Realse Year")]
        public DateTime ReleaseYear { get; set; }
        [Required]
        [Display(Name ="Duration In Hours")]
        public double Duration { get; set; }
        [Required]
        public string Language { get; set; }
        [Required]
        public string Country { get; set; }
        public IEnumerable<int> Generes { get; set; }
        public IEnumerable<SelectListItem> GeneresList { get; set; }
        [Required]
        public IFormFile Video { get; set; }
        public IFormFile Poster { get; set; }
    }
}
