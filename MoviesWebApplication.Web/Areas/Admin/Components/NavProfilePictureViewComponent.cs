using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoviesWebApplication.DAL.IDataRepository;
using MoviesWebApplication.Web.Areas.Admin.Models.UsersController;
using MoviesWebApplication.Web.Constrains;

namespace MoviesWebApplication.Web.Areas.Admin.Components
{
    public class NavProfilePictureViewComponent : ViewComponent
    {
        private readonly IUnitOfWork ufw;
        private readonly IMapper mapper;
        public NavProfilePictureViewComponent(IUnitOfWork ufw, IMapper mapper)
        {
            this.ufw = ufw;
            this.mapper = mapper;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await ufw.Users.GetUserByUserNameIncludesRoleAsync(User.Identity.Name);
            if (string.IsNullOrEmpty(user.ImgUrl)) {
                user.ImgUrl = _Image.Admin; 
            }

            return View(model: mapper.Map<UsersDetailsViewModel>(user));
        }
    }
}
