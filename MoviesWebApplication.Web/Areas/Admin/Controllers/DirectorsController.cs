using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.DAL.IDataRepository;
using MoviesWebApplication.Web.Areas.Admin.Models.DirectorsModels;
using MoviesWebApplication.Web.Constrains;

namespace MoviesWebApplication.Web.Areas.Admin.Controllers
{
    public class DirectorsController : AdminBaseController
    {

        private readonly IUnitOfWork ufw;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment webHostEnvironment;
        public DirectorsController(IUnitOfWork ufw, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            this.ufw = ufw;
            this.mapper = mapper;
            this.webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index(int page=1)
        {
            
            var directors = await ufw.Directors.GetAllDirectorsAsync((page - 1) * _Pagination.PageSize, _Pagination.PageSize);

            var pages = Math.Ceiling(await ufw.Directors.CountDirectorsAsync() / (double)_Pagination.PageSize);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;

            return View(mapper.Map<IEnumerable<DirectorsIndexViewModel>>(directors));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int directorId,string returnUrl)
        {
            var model = mapper.Map<EditDirectorViewModel>(await ufw.Directors.GetDirectorByIdAsync(directorId));

            model.Genders = new List<Gender>
                {
                    Gender.Male,
                    Gender.Female
                }.Select(gender => new SelectListItem { Text = gender.ToString(), Value = gender.ToString() });

            model.ReturnUrl = returnUrl;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditDirectorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var director = await ufw.Directors.GetDirectorByIdAsync(model.Id);
                var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, director.ImgUrl);
                var oldImageUrl = director.ImgUrl;
                string newImagePath = null;
                director = mapper.Map<Director>(model);

                if (model.Image is null || model.Image.ContentType.ToLower().Contains("image"))
                {
                    if (model.Image is not null)
                    {
                        var newImageName = String.Concat(Guid.NewGuid(), Path.GetExtension(model.Image.FileName));
                        var imageUrl = Path.Combine(_Image.DirectorImages, newImageName);
                        newImagePath = Path.Combine(webHostEnvironment.WebRootPath, imageUrl);
                        using (var fileStream = System.IO.File.Create(newImagePath))
                        {
                            model.Image.CopyTo(fileStream);
                        }
                        director.ImgUrl = imageUrl;
                    }
                    else
                    {
                        director.ImgUrl = oldImageUrl;
                    }

                    if (await ufw.Directors.UpdateDirectorAsync(director))
                    {
                        if (model.Image is not null && oldImageUrl != _Image.Director && System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        TempData[_TempData.Success] = "Director Edited Successfully";
                        return Redirect(model.ReturnUrl);
                    }

                    if (System.IO.File.Exists(newImagePath))
                    {
                        System.IO.File.Delete(newImagePath);
                    }

                }
                else
                {
                    TempData[_TempData.Warning] = "Please Choose A Correct Image";
                }

            }

            TempData[_TempData.Danger] = "Failed To Edit The Director";

            model.Genders = new List<Gender>
                {
                    Gender.Male,
                    Gender.Female
                }.Select(gender => new SelectListItem { Text = gender.ToString(), Value = gender.ToString() });

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveDirector(int directorId,string returnUrl)
        {
            var director = await ufw.Directors.GetDirectorByIdAsync(directorId);
            if (director is not null)
            {
                var oldPath = Path.Combine(webHostEnvironment.WebRootPath, director.ImgUrl);
                if (await ufw.Directors.RemoveDirectorById(directorId))
                {
                    if (director.ImgUrl!=_Image.Director&&System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                    TempData[_TempData.Success] = "Director Removed Successfully";
                }
            }
            else
            {
                TempData[_TempData.Danger] = "Falied To Remove An Actor";
            }

            return Redirect(returnUrl);

        }

        [HttpPost, HttpGet]
        public async Task<IActionResult> Search(int page = 1, string searchInput = null)
        {
            var directors = await ufw.Directors.GetAllDirectorsByNameAsync((page - 1) * _Pagination.PageSize, _Pagination.PageSize, searchInput);

            var pages = Math.Ceiling(await ufw.Directors.CountDirectorsByNameAsync(searchInput) / (double)_Pagination.PageSize);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;
            ViewBag.SearchInput = searchInput;

            return View(nameof(Index), mapper.Map<IEnumerable<DirectorsIndexViewModel>>(directors));
        }

        [HttpGet]
        public IActionResult CreateDirector()
        {
            return View(new CreateDirectorViewModel
            {
                Genders = new List<Gender>
                {
                    Gender.Male,
                    Gender.Female
                }.Select(gender => new SelectListItem { Value = gender.ToString(), Text = gender.ToString() })
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateDirector(CreateDirectorViewModel model)
        {
            var isFailed = true;
            var isImage = false;
            if (ModelState.IsValid)
            {
                var director = mapper.Map<Director>(model);
                if (model.Image is null)
                {
                    director.ImgUrl = _Image.Director;
                    isImage = true;
                }
                else if (model.Image.ContentType.ToLower().Contains("image"))
                {
                    var newFileName = string.Concat(Guid.NewGuid(), Path.GetExtension(model.Image.FileName));
                    var url = Path.Combine(_Image.DirectorImages, newFileName);
                    var path = Path.Combine(webHostEnvironment.WebRootPath, url);
                    using (var fileStream = System.IO.File.Create(path))
                    {
                        model.Image.CopyTo(fileStream);
                    }

                    director.ImgUrl = url;
                    isImage = true;

                }

                if (isImage&&await ufw.Directors.AddDirectorAsync(director))
                {
                    TempData[_TempData.Success] = "Director Created Successfully";
                    isFailed = false;
                }
                else if (isImage && director.ImgUrl != _Image.Director)
                {
                    System.IO.File.Delete(Path.Combine(webHostEnvironment.WebRootPath,director.ImgUrl));
                }

                if (isFailed)
                {
                    if (!isImage)
                    {
                        TempData[_TempData.Warning] = "please Upload a correct file Image";
                    }
                    TempData[_TempData.Danger] = "Failed To Create A Director";
                }

            }

            model.Genders = new List<Gender>
                {
                    Gender.Male,
                    Gender.Female
                }.Select(gender => new SelectListItem { Value = gender.ToString(), Text = gender.ToString() });


            return View(model);

        }

        [HttpGet]
        public async Task<IActionResult> Details(int directorId)
        {
            var director = await ufw.Directors.GetDirectorByIdAsync(directorId);
            return View(mapper.Map<DetailsDirectorViewModel>(director));
        }


    }
}
