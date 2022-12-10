using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.DAL.IDataRepository;
using MoviesWebApplication.Web.Areas.Admin.Models.UsersController;
using MoviesWebApplication.Web.Constrains;

namespace MoviesWebApplication.Web.Areas.Admin.Controllers
{
    public class UsersController : AdminBaseController
    { 

        private readonly IUnitOfWork ufw;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IPasswordHasher<User> passwordHasher;

        public UsersController(IUnitOfWork ufw,IMapper mapper,
            IPasswordHasher<User> passwordHasher,
            IWebHostEnvironment webHostEnvironment)
        {
            this.ufw = ufw;
            this.mapper = mapper;
            this.webHostEnvironment = webHostEnvironment;
            this.passwordHasher = passwordHasher;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var pages = Math.Ceiling(await ufw.Users.CountAllUsersAsync() / (double)_Pagination.PageSize);

            var user = await ufw.Users.GetAllUsersIncludesRoleAsync((page - 1) * _Pagination.PageSize, _Pagination.PageSize);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;

            return View(mapper.Map<IEnumerable<UsersDetailsViewModel>>(user));
        }

        [HttpGet]
        public async Task<IActionResult> BlockedUsers(int page=1)
        {
            var pages = Math.Ceiling(await ufw.Users.CountUsersAsync(true) / (double)_Pagination.PageSize);

            var user = await ufw.Users.GetUsersIncludesRoleAsync((page - 1) * _Pagination.PageSize, _Pagination.PageSize,true);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;

            return View(nameof(Index),mapper.Map<IEnumerable<UsersDetailsViewModel>>(user));
        }


        [HttpGet]
        public async Task<IActionResult> UnBlockedUsers(int page = 1)
        {
            var pages = Math.Ceiling(await ufw.Users.CountUsersAsync(false) / (double)_Pagination.PageSize);

            var user = await ufw.Users.GetUsersIncludesRoleAsync((page - 1) * _Pagination.PageSize, _Pagination.PageSize, false);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;

            return View(nameof(Index),mapper.Map<IEnumerable<UsersDetailsViewModel>>(user));
        }

        [HttpGet]
        public async Task<IActionResult> BlockUser(int id,string returnUrl)
        {
            var user = await ufw.Users.GetUserByIdAsync(id);
            user.IsBlocked = true;

            if(!await ufw.Users.UpdateUserAsync(user))
            {
                TempData[_TempData.Danger] = $"Failed To Block {user.FirstName} {user.LastName}";   
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> UnBlockUser(int id, string returnUrl)
        {
            var user = await ufw.Users.GetUserByIdAsync(id);
            user.IsBlocked = false;

            if (!await ufw.Users.UpdateUserAsync(user))
            {
                TempData[_TempData.Danger] = $"Failed To Block {user.FirstName} {user.LastName}";
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> AddANewUser()
        {
            var roles = await ufw.Roles.GetAllRolesAsync();

            return View(new AddANewUserViewModel { 
                Roles=roles.Select(role => new SelectListItem { Value = role.Id.ToString(), Text = role.Name }) 
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddANewUser(AddANewUserViewModel model)
        {
            var isFailed = true;
            var isFileImage = false;
            if (ModelState.IsValid)
            {
                var user = mapper.Map<User>(model);
                user.PasswordHash = passwordHasher.HashPassword(user, model.Password);

                var role = await ufw.Roles.GetRoleByIdAsync(model.RoleId);

                if (model.Image is null)
                {
                    if (role.Name == _Role.Admin)
                    {
                        user.ImgUrl = _Image.Admin;
                    }
                    else
                    {
                        user.ImgUrl = _Image.User;
                    }

                    isFileImage = true;
                }
                else if (model.Image.ContentType.ToLower().Contains("image"))
                {
                    var newFileName = String.Concat(Guid.NewGuid(), Path.GetExtension(model.Image.FileName));
                    var Url = Path.Combine(_Image.AccountImages, newFileName);
                    var path = Path.Combine(webHostEnvironment.WebRootPath, Url);
                    using (var fileStream = System.IO.File.Create(path))
                    {
                        model.Image.CopyTo(fileStream);
                    }

                    user.ImgUrl = Url;
                    isFileImage = true;
                }

                if (isFileImage)
                {
                    user = await ufw.Users.AddUserAsync(user);

                    if (user is not null)
                    {
                        await ufw.Users.AddUserRoleAsync(user, new Role { Id = model.RoleId });
                        TempData[_TempData.Success] = "Account Created Successfully";
                        isFailed = false;
                    }
                }

                if (isFileImage && isFailed && user.ImgUrl != _Image.Admin && user.ImgUrl != _Image.User)
                {
                    System.IO.File.Delete(Path.Combine(webHostEnvironment.WebRootPath, user.ImgUrl));
                }

            }
            if (isFailed)
            {
                if (!isFileImage)
                {
                    TempData[_TempData.Warning] = "please Upload a correct file Image";
                }
                TempData[_TempData.Danger] = "Failed To Register A New Account";
            }

            var roles = await ufw.Roles.GetAllRolesAsync();
            model.Roles = roles.Select(role => new SelectListItem { Value = role.Id.ToString(), Text = role.Name });

            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> RemoveUser(int id,string returnUrl)
        {
            var user = await ufw.Users.GetUserByIdAsync(id);

            if(user is not null)
            {
                var oldPath =Path.Combine(webHostEnvironment.WebRootPath,user.ImgUrl);
                if (!string.IsNullOrEmpty(user.ImgUrl) && user.ImgUrl != _Image.User && user.ImgUrl != _Image.Admin && System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }

                if (await ufw.Users.RemoveUserAsync(user))
                {
                    TempData[_TempData.Success] = "User Removed Successfully";
                }
            }
            else
            {
                TempData[_TempData.Danger] = "Failed To Remove User Successfully";
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeRole(int userId,string returnUrl)
        {
            var roles = await ufw.Roles.GetAllRolesAsync();
            return View(new ChangeRoleViewModel
            {
                UserId = userId,
                Roles = roles.Select(role => new SelectListItem { Value = role.Id.ToString(), Text = role.Name }),
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole(ChangeRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await ufw.Users.GetUserByIdAsync(model.UserId);
                if(user is not null)
                {
                    var role = await ufw.Roles.GetRoleByIdAsync(model.RoleId);
                    if(role is not null)
                    {
                        var result = await ufw.Users.ChangeUserRoleAsync(user, role);
                        if(result == true)
                        {
                            TempData[_TempData.Success] = $"{user.FirstName} {user.LastName} Role Changed Successfully";
                            return Redirect(model.ReturnUrl);
                        }
                    }

                    TempData[_TempData.Danger] = $"Failed To Change {user.FirstName} {user.LastName} Role";
                }
                
            }
            else
            {
                TempData[_TempData.Success] = "Failed To Change Role User Not Found";
            }
            
            var roles = await ufw.Roles.GetAllRolesAsync();
            model.Roles = roles.Select(role => new SelectListItem { Value = role.Id.ToString(), Text = role.Name });
            return View(model);
        }

        [HttpPost,HttpGet]
        public async Task<IActionResult> Search(int page = 1, string searchInput = null)
        {
            if (String.IsNullOrEmpty(searchInput))
            {
                return RedirectToPage(nameof(Index));
            }
            var users = await ufw.Users.FindUsersByEmailIncludesRoleAsync((page - 1) * _Pagination.PageSize, _Pagination.PageSize, searchInput);

            var pages = Math.Ceiling(await ufw.Users.CountUsersWhereEmailAsync(searchInput)/(double)_Pagination.PageSize);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;
            ViewBag.SearchInput = searchInput;

            return View(nameof(Index),mapper.Map<IEnumerable<UsersDetailsViewModel>>(users));
        }

    }
}
