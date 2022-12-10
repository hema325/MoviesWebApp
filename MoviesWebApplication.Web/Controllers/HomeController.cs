using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.DAL.DataBaseManagement;
using MoviesWebApplication.DAL.IDataRepository;
using MoviesWebApplication.DLL.IDataRepository;
using MoviesWebApplication.Web.Constrains;
using MoviesWebApplication.Web.Models;
using MoviesWebApplication.Web.Models.MoviesModels;
using MoviesWebApplication.Web.WebOptions;
using System.Diagnostics;
using System.Security.Claims;

namespace MoviesWebApplication.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork ufw;
        private readonly IMapper mapper;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, 
            IUnitOfWork ufw,
            IMapper mapper)
        {
            this.ufw = ufw;
            this.mapper = mapper;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var movies = await ufw.Movies.GetAllMoviesHeighstRatedAsync(0, _Pagination.PageSize);
            return View(mapper.Map<IEnumerable<MoviesIndexViewModel>>(movies));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}