using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesWebApplication.DAL.IDataRepository;
using MoviesWebApplication.Web.Constrains;
using MoviesWebApplication.Web.Models.MoviesModels;

namespace MoviesWebApplication.Web.Controllers
{
    [Authorize]
    public class MoviesController : Controller
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork ufw;

        public MoviesController(IMapper mapper, IUnitOfWork ufw)
        {
            this.mapper = mapper;
            this.ufw = ufw;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(int page=1)
        {
            var movies = await ufw.Movies.GetAllMoviesAsync((page - 1) * _Pagination.PageSize, _Pagination.PageSize);

            var pages = Math.Ceiling(await ufw.Movies.CountMoviesAsync() / (double)_Pagination.PageSize);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;

            return View(mapper.Map<IEnumerable<MoviesIndexViewModel>>(movies));
        }

        [AllowAnonymous]
        public async Task<IActionResult> Search(int page = 1,string searchInput=null)
        {
            var movies = await ufw.Movies.GetAllMoviesByTitleAsync((page - 1) * _Pagination.PageSize, _Pagination.PageSize,searchInput);

            var pages = Math.Ceiling(await ufw.Movies.CountMoviesByTitleAsync(searchInput) / (double)_Pagination.PageSize);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;
            ViewBag.SearchInput = searchInput;

            return View(nameof(Index),mapper.Map<IEnumerable<MoviesIndexViewModel>>(movies));
        }

        public async Task<IActionResult> Watch(int Id)
        {
            var movie = await ufw.Movies.GetMovieByIdAsync(Id);
            var movieGeneres = await ufw.Generes.GetAllGeneresForMovieAsync(Id);
            var movieActors = await ufw.Actors.GetAllActorsForMovieAsync(Id);
            var movieDirectors = await ufw.Directors.GetAllDirectorsForMovieAsync(Id);
            var model = mapper.Map<WatchMovieViewModel>(movie);
            model.Generes = movieGeneres.Select(genere => genere.Name);
            model.Actors = mapper.Map<IEnumerable<MovieActorsViewModel>>(movieActors);
            model.Directors = mapper.Map < IEnumerable<MovieDirectorsViewModel>>(movieDirectors);

            return View(model);
        }

    }
}
