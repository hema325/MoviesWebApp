using Microsoft.AspNetCore.Mvc;

namespace MoviesWebApplication.Web.Areas.Admin.Controllers
{
    public class HomeController : AdminBaseController
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult SearchProject(string projectName)
        {
            return RedirectToAction("Index", projectName);
        }

    }
}
