using Microsoft.AspNetCore.Mvc.Rendering;
using MoviesWebApplication.Web.Areas.Identity.Models;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Admin.Models.UsersController
{
    public class AddANewUserViewModel : RegisterViewModel
    {
        [Display(Name = "Upload Your Image")]
        public IFormFile Image { get; set; }
        [Required]
        public int RoleId { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }

    }
}
