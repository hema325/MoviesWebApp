using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoviesWebApplication.DAL.IDataRepository;
using MoviesWebApplication.Web.Constrains;
using System.Security.Claims;

namespace MoviesWebApplication.Web.ViewComponents
{
    public class UserViewComponent:ViewComponent
    {
        private readonly IUnitOfWork ufw;
        public UserViewComponent(IUnitOfWork ufw)
        {
            this.ufw = ufw;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await ufw.Users.GetUserByUserNameAsync(User.Identity.Name);
            if (user is not null)
            {
                return View(model: user.ImgUrl);
            }

            return View(model: _Image.User);
        }
    }
}
