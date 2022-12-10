using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.DAL.IDataRepository;
using MoviesWebApplication.Web.Areas.Admin.Models.ActorsModels;
using MoviesWebApplication.Web.Areas.Admin.Models.MoviesModels;
using MoviesWebApplication.Web.Constrains;

namespace MoviesWebApplication.Web.Areas.Admin.Controllers
{
    public class MoviesController : AdminBaseController
    {
        private readonly IUnitOfWork ufw;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment webHostEnvironment;
        public MoviesController(IUnitOfWork ufw, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            this.ufw = ufw;
            this.mapper = mapper;
            this.webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            var movies = await ufw.Movies.GetAllMoviesAsync((page - 1) * _Pagination.PageSize, _Pagination.PageSize);
            var pages = Math.Ceiling(await ufw.Movies.CountMoviesAsync() / (double)_Pagination.PageSize);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;

            return View(mapper.Map<IEnumerable<AdminMoviesIndexViewModel>>(movies));
        }

        [HttpGet]
        public async Task<IActionResult> CreateMovie()
        {
            var generes = await ufw.Generes.GetAllGeneresAsync();
            return View(new CreateMovieViewModel
            {
                GeneresList=generes.Select(genere=>new SelectListItem { Text=genere.Name,Value=genere.Id.ToString()})
            });
        }

        [HttpPost]
        [RequestSizeLimit(long.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit =long.MaxValue)]
        public async Task<IActionResult> CreateMovie(CreateMovieViewModel model)
        {
            var isFailed = true;
            if (ModelState.IsValid)
            {
                var movie = mapper.Map<Movie>(model);

                if (model.Video is not null && model.Video.ContentType.ToLower().Contains("video")) 
                {
                    if (model.Poster is null || !model.Poster.ContentType.ToLower().Contains("image")) 
                    {
                        movie.MoviePosterUrl = _Image.Poster;
                    }
                    else
                    {
                        var newPosterName = String.Concat(Guid.NewGuid(),Path.GetExtension(model.Poster.FileName));
                        var posterUrl = Path.Combine(_Image.PosterImages, newPosterName);
                        var posterPath = Path.Combine(webHostEnvironment.WebRootPath, posterUrl);
                        using (var fileStream = System.IO.File.Create(posterPath))
                        {
                            model.Poster.CopyTo(fileStream);
                        }
                        movie.MoviePosterUrl = posterUrl;
                    }

                    var newVideoName = String.Concat(Guid.NewGuid(), Path.GetExtension(model.Video.FileName));
                    var videoUrl = Path.Combine(_Image.Vidoes, newVideoName);
                    var videoPath = Path.Combine(webHostEnvironment.WebRootPath, videoUrl);
                    using (var fileStream = System.IO.File.Create(videoPath))
                    {
                        model.Video.CopyTo(fileStream);
                    }
                    movie.MovieUrl = videoUrl;

                    if(await ufw.Movies.AddMovieAsync(movie))
                    {
                        var addedMovie = await ufw.Movies.GetMovieByTitleAsync(model.Title);
                        await ufw.MoviesGeneres.AddMoviesGeneresAsync(model.Generes.Select(num => new MovieGenere { GenereId = num, MovieId = addedMovie.Id }));
                        TempData[_TempData.Success] = "Movie Added Successfully";
                        isFailed = false;
                    }
                    else
                    {
                        var posterPath= Path.Combine(webHostEnvironment.WebRootPath, movie.MoviePosterUrl);
                        if (System.IO.File.Exists(posterPath))
                        {
                            System.IO.File.Delete(posterPath);
                        }
                        
                        if(System.IO.File.Exists(videoPath))
                        {
                            System.IO.File.Delete(videoPath);
                        }
                    }

                }

            }

            if (isFailed)
            {
                TempData[_TempData.Danger] = "Failed To Add A Movie";
            }

            var generes = await ufw.Generes.GetAllGeneresAsync();
            model.GeneresList = generes.Select(genere => new SelectListItem { Text = genere.Name, Value = genere.Id.ToString() });

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int movieId,string returnUrl)
        {
            var movie = await ufw.Movies.GetMovieByIdAsync(movieId);
            var model = mapper.Map<EditMovieViewModel>(movie);
            model.returnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [RequestSizeLimit(long.MaxValue)]
        public async Task<IActionResult> Edit(EditMovieViewModel model)
        {
            if (ModelState.IsValid)
            {
                var movie = await ufw.Movies.GetMovieByIdAsync(model.Id);
                var oldPosterPath = Path.Combine(webHostEnvironment.WebRootPath, movie.MoviePosterUrl);
                var oldPosterUrl = movie.MoviePosterUrl;
                var videoUrl = movie.MovieUrl;
                string newPosterPath = null;
                movie = mapper.Map<Movie>(model);
                if (model.Poster is null || model.Poster.ContentType.ToLower().Contains("image"))
                {
                    if (model.Poster is not null)
                    {
                        var newPosterName = String.Concat(Guid.NewGuid(), Path.GetExtension(model.Poster.FileName));
                        var posterUrl = Path.Combine(_Image.PosterImages, newPosterName);
                        newPosterPath = Path.Combine(webHostEnvironment.WebRootPath, posterUrl);
                        using (var fileStream = System.IO.File.Create(newPosterPath))
                        {
                            model.Poster.CopyTo(fileStream);
                        }
                        movie.MoviePosterUrl = posterUrl;
                    }
                    else
                    {
                        movie.MoviePosterUrl = oldPosterUrl;
                    }

                    if (await ufw.Movies.UpdateMovieAsync(movie))
                    {
                        if (model.Poster is not null && oldPosterUrl != _Image.Poster && System.IO.File.Exists(oldPosterPath))
                        {
                            System.IO.File.Delete(oldPosterPath);
                        }
                        TempData[_TempData.Success] = "Movie Edited Successfully";
                        return Redirect(model.returnUrl);
                    }

                    if (System.IO.File.Exists(newPosterPath))
                    {
                        System.IO.File.Delete(newPosterPath);
                    }
                }
                else
                {
                    TempData[_TempData.Warning] = "please Choose A Correct Image";
                }
            }

            TempData[_TempData.Danger] = "Failed To Edit the Movie";

            return View(model);
        }

        [HttpPost, HttpGet]
        public async Task<IActionResult> Search(int page = 1, string searchInput = null)
        {
            var actors = await ufw.Movies.GetAllMoviesByTitleAsync((page - 1) * _Pagination.PageSize, _Pagination.PageSize, searchInput);

            var pages = Math.Ceiling(await ufw.Movies.CountMoviesByTitleAsync(searchInput) / (double)_Pagination.PageSize);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;
            ViewBag.SearchInput = searchInput;

            return View(nameof(Index), mapper.Map<IEnumerable<AdminMoviesIndexViewModel>>(actors));
        }

        [HttpGet]
        public async Task<IActionResult> RemoveMovie(int id,int page,string returnUrl)
        {
            var movie = await ufw.Movies.GetMovieByIdAsync(id);
            
            if(movie is not null&&await ufw.Movies.RemoveMovieById(movie.Id))
            {
                if(movie.MoviePosterUrl != _Image.Poster)
                {
                    var oldPosterPath = Path.Combine(webHostEnvironment.WebRootPath, movie.MoviePosterUrl);
                    if (System.IO.File.Exists(oldPosterPath))
                    {
                        System.IO.File.Delete(oldPosterPath);
                    }
                }

                var oldMoviePath = Path.Combine(webHostEnvironment.WebRootPath, movie.MovieUrl);
                if (System.IO.File.Exists(oldMoviePath))
                {
                    System.IO.File.Delete(oldMoviePath);
                }

                TempData[_TempData.Success] = "Movie Removed Successfully";
            }
            else
            {
                TempData[_TempData.Danger] = "Failed To Remove Movie";
            }

            return Redirect(returnUrl);

        }

        [HttpGet]
        public async Task<IActionResult> Details(int movieId)
        {
            var model = mapper.Map<DetailsMovieViewModel>(await ufw.Movies.GetMovieByIdAsync(movieId));
            model.Actors= mapper.Map<IEnumerable<ActorsIncludeMoviesActorsViewModel>>(await ufw.Actors.GetAllActorsForMovieAsync(movieId));
            model.Directors = mapper.Map<IEnumerable<DirectorsIncludeMoviesDirectorsViewModel>>(await ufw.Directors.GetAllDirectorsForMovieAsync(movieId));
            model.Reviewers = mapper.Map<IEnumerable<ReviewersIncludeMoviesReviewersViewModel>>(await ufw.Reviewers.GetAllReviewersForMovieAsync(movieId));
            model.Generes = mapper.Map<IEnumerable<GeneresIncludeMoviesGeneresViewModel>>(await ufw.Generes.GetAllGeneresForMovieAsync(movieId));
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveMovieActor(int movieId,int actorId,string returnUrl)
        {
            if (await ufw.MoviesActors.RemoveMovieActorAsync(new MovieActor { ActorId = actorId, MovieId = movieId }))
            {
                TempData[_TempData.Success] = "Movie Actor Removed Successfully";

            }
            else
            {
                TempData[_TempData.Danger] = "Failed To Remove Movie Actor";
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveMovieDirector(int movieId, int directorId, string returnUrl)
        {
            if (await ufw.MoviesDirectors.RemoveMovieDirectorAsync(new MovieDirector { DirectorId = directorId, MovieId = movieId }))
            {
                TempData[_TempData.Success] = "Movie Director Removed Successfully";

            }
            else
            {
                TempData[_TempData.Danger] = "Failed To Remove Movie Director";
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveMovieReviewer(int movieId, int reviewerId, string returnUrl)
        {
            if (await ufw.MoviesReviewers.RemoveMovieReviewerAsync(new MovieReviewer { ReviewerId = reviewerId, MovieId = movieId }))
            {
                TempData[_TempData.Success] = "Movie Reviewer Removed Successfully";

            }
            else
            {
                TempData[_TempData.Danger] = "Failed To Remove Movie Reviewer";
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveMovieGenere(int movieId, int genereId, string returnUrl)
        {
            if (await ufw.MoviesGeneres.RemoveMovieGenereAsync(new MovieGenere { GenereId = genereId, MovieId = movieId }))
            {
                TempData[_TempData.Success] = "Movie Reviewer Removed Successfully";

            }
            else
            {
                TempData[_TempData.Danger] = "Failed To Remove Movie Reviewer";
            }

            return Redirect(returnUrl);
        }

        [HttpGet]
        public async Task<IActionResult> AddMovieGenere(int movieId,string returnUrl)
        {
            var genres = await ufw.Generes.GetAllGeneresAsync();
            return View(new AddMovieGenereViewModel
            {
                MovieId = movieId,
                GeneresList = genres.Select(genere=>new SelectListItem { Value=genere.Id.ToString(),Text=genere.Name}),
                returnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddMovieGenere(AddMovieGenereViewModel model)
        {
            var movieGenere = mapper.Map<MovieGenere>(model);
            var isFailed = true;
            if (ModelState.IsValid)
            {
                if(await ufw.MoviesGeneres.GettMovieGenereAsync(movieGenere.MovieId, movieGenere.GenereId) is null)
                {
                    if(await ufw.MoviesGeneres.AddMovieGenereAsync(movieGenere))
                    {
                        TempData[_TempData.Success] = "Movie Genere Added Successfully";
                        isFailed = false;
                    }
                }
                else
                {
                    TempData[_TempData.Warning] = "Movie Genere Already Exists";
                }
            }

            if (isFailed)
            {
                TempData[_TempData.Danger] = "Failed To Add Movie Genere";
            }

            var genres = await ufw.Generes.GetAllGeneresAsync();
            model.GeneresList = genres.Select(genere => new SelectListItem { Value = genere.Id.ToString(), Text = genere.Name });
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddMovieDirector(int movieId,string returnUrl)
        {
            var directors = await ufw.Directors.GetAllDirectorsAsync();
            return View(new AddMovieDirectorViewModel
            {
                MovieId=movieId,
                DirectorsList = directors.Select(director=>new SelectListItem { Text=$"{director.FirstName} {director.LastName}",Value=director.Id.ToString()}),
                returnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddMovieDirector(AddMovieDirectorViewModel model)
        {
            var movieDirector = mapper.Map<MovieDirector>(model);
            var isFailed = true;

            if (ModelState.IsValid && movieDirector is not null)
            {
                if (await ufw.MoviesDirectors.GetMovieDirectorAsync(model.MovieId, model.DirectorId) is null)
                {
                    if (await ufw.MoviesDirectors.AddMovieDirectorAsync(movieDirector))
                    {
                        TempData[_TempData.Success] = "Movie Director is Added Successfully";
                        isFailed = false;
                    }
                }
                else
                {
                    TempData[_TempData.Warning] = "Movie Director Already Exists";
                }
            }

            if (isFailed)
            {
                TempData[_TempData.Danger] = "Failed To Add Movie Director";
            }


            var Directors = await ufw.Directors.GetAllDirectorsAsync();
            model.DirectorsList = Directors.Select(Director => new SelectListItem { Text = $"{Director.FirstName} {Director.LastName}", Value = Director.Id.ToString() });

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddMovieReviewer(int movieId,string returnUrl)
        {
            var reviwers = await ufw.Reviewers.GetAllReviewersAsync();
            return View(new AddMovieReviewerViewModel
            {
                MovieId = movieId,
                ReviewersList = reviwers.Select(reviewer => new SelectListItem { Text = $"{reviewer.FirstName} {reviewer.LastName}", Value = reviewer.Id.ToString() }),
                returnUrl = returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddMovieReviewer(AddMovieReviewerViewModel model)
        {
            var movieReviewer = mapper.Map<MovieReviewer>(model);
            var isFailed = true;

            if (ModelState.IsValid && movieReviewer is not null) {
                if (await ufw.MoviesReviewers.GetMovieReviewerAsync(model.MovieId,model.ReviewerId) is null)
                {
                    if (await ufw.MoviesReviewers.AddMovieReviewerAsync(movieReviewer))
                    {
                        TempData[_TempData.Success] = "Movie Reviewer is Added Successfully";
                        isFailed = false;
                    }
                }
                else
                {
                    TempData[_TempData.Warning] = "Movie Reviewer Already Exists";
                }
            }

            if (isFailed)
            {
                TempData[_TempData.Danger] = "Failed To Add Movie Reviewer";
            }


            var reviwers = await ufw.Reviewers.GetAllReviewersAsync();
            model.ReviewersList = reviwers.Select(reviewer => new SelectListItem { Text = $"{reviewer.FirstName} {reviewer.LastName}", Value = reviewer.Id.ToString() });
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AddMovieActor(int movieId,string returnUrl)
        {
            var actors = await ufw.Actors.GetAllActorsAsync();
            return View(new AddMovieActorViewModel
            {
                MovieId=movieId,
                ActorsList=actors.Select(actor=>new SelectListItem { Value=actor.Id.ToString(),Text=$"{actor.FirstName} {actor.LastName}"}),
                returnUrl=returnUrl
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddMovieActor(AddMovieActorViewModel model)
        {

            var movieActor = mapper.Map<MovieActor>(model);
            var isFailed = true;

            if (ModelState.IsValid && movieActor is not null)
            {

                if (await ufw.MoviesActors.GetMovieActorAsync(movieActor.MovieId, movieActor.ActorId) is null)
                {
                    if (await ufw.MoviesActors.AddMovieActorAsync(movieActor))
                    {
                        TempData[_TempData.Success] = "Movie Actor Added Successfully";
                        isFailed = false;
                    }
                }
                else
                {
                    TempData[_TempData.Warning] = "Movie Actor Already Exists";
                }
            }

            if (isFailed)
            {
                TempData[_TempData.Danger] = "Failed To Add Movie Actor";
            }

            var actors = await ufw.Actors.GetAllActorsAsync();
            model.ActorsList = actors.Select(actor => new SelectListItem { Value = actor.Id.ToString(), Text = $"{actor.FirstName} {actor.LastName}" });

            return View(model);
        }

    }
}
