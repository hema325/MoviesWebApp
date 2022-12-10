using System.ComponentModel.DataAnnotations;

namespace MoviesWebApplication.Web.Areas.Admin.Models.ActorsModels
{
    public class EditActorViewModel:CreateActorViewModel
    {
        [Required]
        public int Id { get; set; }
        public string ReturnUrl { get; set; }
    }
}
