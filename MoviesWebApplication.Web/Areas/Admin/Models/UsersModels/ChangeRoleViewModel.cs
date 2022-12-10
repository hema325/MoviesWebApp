using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Admin.Models.UsersController
{
    public class ChangeRoleViewModel
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        [Display(Name = "Choose A Role")]
        public int RoleId { get; set; }
        public string ReturnUrl { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
    }
}
