using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Admin.Models.MoviesModels
{
    public class AddMovieReviewerViewModel
    {
        [Required]
        public int MovieId { get; set; }
        [Required]
        public int ReviewerId { get; set; }
        [Required]
        [Range(0,5)]
        public int Stars { get; set; }

        public IEnumerable<SelectListItem> ReviewersList { get; set; }

        public string returnUrl { get; set; }
    }
}
