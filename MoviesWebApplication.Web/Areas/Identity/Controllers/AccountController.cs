using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.Web.Areas.Identity.Models;
using MoviesWebApplication.Web.Constrains;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using MoviesWebApplication.Web.Services.Email;
using System.Net.Mail;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.DataProtection;
using MoviesWebApplication.DAL.IDataRepository;
using Microsoft.AspNetCore.Authorization;

namespace MoviesWebApplication.Web.Areas.Identity.Controllers
{
    [Area(_Area.Identity)]
    public class AccountController : Controller
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork ufw;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly IEmailSender emailSender;
        private readonly IDataProtector protector;
        private readonly IWebHostEnvironment webHostEnvironment;
        public AccountController(IMapper mapper,IUnitOfWork ufw,
            IPasswordHasher<User> passwordHasher,
            IEmailSender emailSender,
            IDataProtectionProvider provider,
            IWebHostEnvironment webHostEnvironment)
        {
            protector = provider.CreateProtector(String.Empty);
            this.mapper = mapper;
            this.passwordHasher = passwordHasher;
            this.ufw = ufw;
            this.emailSender = emailSender;
            this.webHostEnvironment = webHostEnvironment;
        }

        private ClaimsPrincipal CreateUserPrincipal(User user,string Scheme,string Role = null)
        {
            if (string.IsNullOrEmpty(Role))
                Role = _Role.User;

            var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                        new Claim(ClaimTypes.Name,user.UserName),
                        new Claim(ClaimTypes.Role,Role)
                    };
            var identity = new ClaimsIdentity(claims,Scheme);
            return new ClaimsPrincipal(identity);
        }

        private async Task SendEmailTokens(string Email, string Url)
        {
            var mailMessage = new MailMessage
            {
                Subject = "EmailVerification",
                Body = $"Pleasse <a href=\"{Url}\" >Click Here</a> To Confirm Your Email Confirm",
                IsBodyHtml = true,
            };

            mailMessage.To.Add(Email);
            await emailSender.SendMailAsync(mailMessage);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!await ufw.Users.CheckIfUserEmailExists(model.Email))
                {
                    var user = mapper.Map<User>(model);
                    user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
                    user.ImgUrl = _Image.User;
                    user = await ufw.Users.AddUserAsync(user);

                    if (user is not null)
                    {
                        var role = await ufw.Roles.GetRoleByNameAsync(_Role.User);
                        await ufw.Users.AddUserRoleAsync(user,role);
                        var Token = Guid.NewGuid().ToString();

                        var result = await ufw.UserTokens.AddUserTokenAsync(user, Token);

                        if (result == true)
                        {
                            try
                            {
                                var AppUrl = Url.Action(nameof(EmailConfirmation), "Account", new { Id = user.Id, Token = Token }, Request.Scheme);

                                await SendEmailTokens(model.Email, AppUrl);

                                TempData[_TempData.Info] = "Verification Message Sent Successfully To Your Email Please Confirm";

                            }
                            catch
                            {
                                await ufw.UserTokens.RemoveUserTokenAsync(user, Token);
                                TempData[_TempData.Danger] = "Faild To Send Verification Method";
                            }
                            TempData[_TempData.Success] = "Account Created Successfully";
                            return RedirectToAction("Index", "Home", new { area = "" });

                        }
                        await ufw.Users.RemoveUserAsync(user);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "This Email has been  Already Taken");
                }
            }

            TempData[_TempData.Danger] = "Failed To Register";
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EmailConfirmation([Required]int Id,[Required]string Token)
        {
            var user = await ufw.Users.GetUserByIdAsync(Id);
            if (ModelState.IsValid && await ufw.UserTokens.GetUserTokenAsync(user,Token) is not null)
            {
                user.EmailConfirmed = true;
                var result = await ufw.Users.UpdateUserAsync(user);
                if(result == true)
                {
                    await ufw.UserTokens.RemoveUserTokenAsync(user, Token);
                    await HttpContext.SignInAsync(_Scheme.Default, CreateUserPrincipal(user,_Scheme.Default));
                    TempData[_TempData.Success] = "Your Email Has Been Confirmed";
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }

            TempData[_TempData.Danger] = "Failed To Confirm";

            return NotFound();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await ufw.Users.GetUserByUserNameIncludesRoleAsync(model.Email);

                if (user is not null && passwordHasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Success)
                {
                    if (!user.IsBlocked)
                    {
                        if (user.EmailConfirmed == true)
                        {
                            await HttpContext.SignInAsync(_Scheme.Default, CreateUserPrincipal(user, _Scheme.Default, user.Role.Name), new AuthenticationProperties
                            {
                                IsPersistent = model.IsPersistent,
                                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                            });

                            TempData[_TempData.Success] = "Logged In Succefully";

                            if (!string.IsNullOrEmpty(model.ReturnUrl))
                                return LocalRedirect(model.ReturnUrl);

                            return RedirectToAction("Index", "Home", new { area = "" });
                        }

                        var token = Guid.NewGuid().ToString();
                        var AppUrl = Url.Action(nameof(EmailConfirmation), "Account", new { token = token, Id = user.Id }, Request.Scheme);

                        var result = await ufw.UserTokens.AddUserTokenAsync(user, token);
                        if (result == true)
                        {
                            await SendEmailTokens(user.Email, AppUrl);
                            TempData[_TempData.Success] = "Verification Message Sent To Your Email Please Confirm Your Email";
                            return RedirectToAction("Index", "Home", new { area = "" });
                        }
                    }
                    else
                    {
                        TempData[_TempData.Warning] = "Your Are Blocked";
                    }
                }
            }

            TempData[_TempData.Danger] = "Failed To Login";

            return View(model);
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(_Scheme.Default);
            TempData[_TempData.Success] = "Logged out Succefully";
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            if (User.IsInRole(_Role.Admin))
            {
                return RedirectToAction(nameof(ConfirmChangePassword), new {Id= Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value)});
            }
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await ufw.Users.GetUserByUserNameAsync(model.Email);
                if(user is not null)
                {
                    var token = Guid.NewGuid().ToString();
                    var AppUrl = Url.Action(nameof(ConfirmChangePassword), "Account", new {Id=user.Id,Token=token},Request.Scheme);
                    var result = await ufw.UserTokens.AddUserTokenAsync(user, token);
                    if (result == true)
                    {
                        await SendEmailTokens(user.Email, AppUrl);
                        TempData[_TempData.Success] = "Message Sent Succefully To Your Email Please Verify";
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }
            }

            TempData[_TempData.Danger] = "Failed To Send Verification Message To Your Email";

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ConfirmChangePassword(int Id,string token)
        {
            var user = await ufw.Users.GetUserByIdIncludesRoleAsync(Id);
            if(user is not null)
            {
                if(user.Role.Name == _Role.Admin)
                {
                    return View(new ConfirmChangePasswordViewModel { Id = Id });
                }
                var result = await ufw.UserTokens.GetUserTokenAsync(user, token);
                if (result is not null)
                {
                    await ufw.UserTokens.RemoveUserTokenAsync(user, result.Token);
                    return View(new ConfirmChangePasswordViewModel { Id=Id});
                }
            }

            return NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ConfirmChangePassword(ConfirmChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await ufw.Users.GetUserByIdAsync(model.Id);

                if (user is not null)
                {
                    user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
                    var result = await ufw.Users.UpdateUserAsync(user);
                    if (result == true)
                    {
                        await HttpContext.SignOutAsync(_Scheme.Default);
                        TempData[_TempData.Success] = "Password Changed Succefully";
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }
            }

            TempData[_TempData.Danger] = "Failed To Change Your Password";

            return View(model);
        }

        [HttpGet]
        public IActionResult ResetPassword()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var Faild = true;

            if (ModelState.IsValid)
            {
                var user = await ufw.Users.GetUserByUserNameAsync(model.Email);
                if (user is not null)
                {
                    var token = Guid.NewGuid().ToString();
                    var AppUrl = Url.Action(nameof(ConfirmResetPassword), "Account", new { Id = user.Id, token = token }, Request.Scheme);
                    var result = await ufw.UserTokens.AddUserTokenAsync(user, token);
                    if (result == true)
                    {
                        await SendEmailTokens(user.UserName, AppUrl);
                        TempData[_TempData.Success] = "Message Sent Succefully To Your Email Please Verify";
                        Faild = false;
                    }
                }

            }
            if(Faild)
            {
                TempData[_TempData.Danger] = "Failed To Send Verification Message To Your Email";
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmResetPassword(int id,string token)
        {
            var user = await ufw.Users.GetUserByIdAsync(id);

            if(user is not null)
            {
                var result = await ufw.UserTokens.GetUserTokenAsync(user, token);
                if(result is not null)
                {
                    await ufw.UserTokens.RemoveUserTokenAsync(user, token);
                    return View(new ConfirmResetPasswordViewModel { Id=user.Id});
                }
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmResetPassword(ConfirmResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await ufw.Users.GetUserByIdAsync(model.Id);

                if (user is not null)
                {
                    user.PasswordHash = passwordHasher.HashPassword(user, model.Password);
                    var result = await ufw.Users.UpdateUserAsync(user);
                    if (result == true)
                    {
                        TempData[_TempData.Success] = "Password Reset Sucessfully";
                        return RedirectToAction("Index", "Home", new { area = "" });
                    }
                }
            }

            TempData[_TempData.Success] = "Failed To Reset Your Password";
            return View(model);
        }

        [HttpGet]
        public IActionResult ExternalLogin(string provider,string returnUrl)
        {
            return new ChallengeResult(provider, new AuthenticationProperties
            {
                RedirectUri = Url.Action(nameof(ExternalLoginCallBack), new { returnUrl = returnUrl })
            });
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallBack(string returnUrl)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(_Scheme.Default);
            if (!authenticateResult.Succeeded)
            {
                TempData[_TempData.Danger] = "Failed To Sign In";
                return RedirectToAction(nameof(Login));
            }

            //var claims = new List<Claim>
            //    {
            //        authenticateResult.Principal.FindFirst(ClaimTypes.Email),
            //        authenticateResult.Principal.FindFirst(ClaimTypes.NameIdentifier)
            //    };

            //var identity = new ClaimsIdentity(claims, _Scheme.External);
            //var user = new ClaimsPrincipal(identity);

            //await HttpContext.SignInAsync(_Scheme.External, user);

            TempData[_TempData.Success] = "Logged In Successfully";

            if (!string.IsNullOrEmpty(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction("Index", "Home", new { area = "" });
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangeImage()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangeImage(ChangeImageViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Image.Name.ToLower().Contains("image"))
                {
                    var user = await ufw.Users.GetUserByUserNameAsync(User.Identity.Name);

                    if (user is not null)
                    {
                        var oldPath = Path.Combine(webHostEnvironment.WebRootPath, user.ImgUrl);
                        if (!string.IsNullOrEmpty(user.ImgUrl) && user.ImgUrl != _Image.Admin && user.ImgUrl != _Image.User && System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }

                        var newFileName = string.Concat(Guid.NewGuid().ToString(), Path.GetExtension(model.Image.FileName));
                        var Url = Path.Combine(_Image.AccountImages, newFileName);
                        var path = Path.Combine(webHostEnvironment.WebRootPath, Url);

                        using (var fileStream = System.IO.File.Create(path))
                        {
                            await model.Image.CopyToAsync(fileStream);
                        }

                        user.ImgUrl = Url;
                        var result = await ufw.Users.UpdateUserAsync(user);

                        if (result == true)
                        {
                            TempData[_TempData.Success] = "Your Image has been Changed Successfully";
                            return RedirectToAction("Index", "Home", new { area = "" });
                        }

                        System.IO.File.Delete(path);

                    }
                    else
                    {
                        TempData[_TempData.Warning] = "You are logged in using External Provider Service is not Available";
                    }
                }
                else
                {
                    TempData[_TempData.Warning] = "Please Choose A Correct Image";
                }
            }

            TempData[_TempData.Danger] = "Faild To Change Your Image";

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
