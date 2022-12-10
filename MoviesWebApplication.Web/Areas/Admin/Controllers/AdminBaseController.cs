using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesWebApplication.Web.Constrains;

namespace MoviesWebApplication.Web.Areas.Admin.Controllers
{
    [Area(_Area.Admin)]
    [Authorize(Roles=_Role.Admin)]
    public abstract class AdminBaseController : Controller { }
}
