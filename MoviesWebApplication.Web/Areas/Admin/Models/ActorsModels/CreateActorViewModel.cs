using Microsoft.AspNetCore.Mvc.Rendering;
using MoviesWebApplication.DAL.Data;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Admin.Models.ActorsModels
{
    public class CreateActorViewModel
    {
        [Required]
        [Display(Name ="First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        public Gender Gender { get; set; }
        public string Biography { get; set; }
        public  IFormFile Image { get; set; }
        public IEnumerable<SelectListItem> Genders { get; set; }
    }
}
