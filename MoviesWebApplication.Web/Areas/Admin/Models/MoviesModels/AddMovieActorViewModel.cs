using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Admin.Models.MoviesModels
{
    public class AddMovieActorViewModel
    {
        [Required]
        public int ActorId { get; set; }
        [Required]
        public int MovieId { get; set; }
        [Required]
        public string Role { get; set; }
        public IEnumerable<SelectListItem> ActorsList { get; set; }
        public string returnUrl { get; set; }
    }
}
