using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Admin.Models.MoviesModels
{
    public class AddMovieGenereViewModel
    {
        [Required]
        public int MovieId { get; set; }
        [Required]
        public int GenereId { get; set; }
        public IEnumerable<SelectListItem> GeneresList { get; set; }
        public string returnUrl { get; set; }
    }
}
