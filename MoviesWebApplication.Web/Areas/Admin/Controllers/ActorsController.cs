using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.DAL.IDataRepository;
using MoviesWebApplication.Web.Areas.Admin.Models.ActorsModels;
using MoviesWebApplication.Web.Constrains;

namespace MoviesWebApplication.Web.Areas.Admin.Controllers
{
    public class ActorsController : AdminBaseController
    {
        private readonly IUnitOfWork ufw;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ActorsController(IUnitOfWork ufw,IMapper mapper,IWebHostEnvironment webHostEnvironment)
        {
            this.ufw = ufw;
            this.mapper = mapper;
            this.webHostEnvironment = webHostEnvironment; 
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var actors = await ufw.Actors.GetAllActorsAsync((page-1)*_Pagination.PageSize,_Pagination.PageSize);

            var pages = Math.Ceiling(await ufw.Actors.CountActorsAsync() / (double)_Pagination.PageSize);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;

            return View(mapper.Map<IEnumerable<ActorsIndexViewModel>>(actors));
        }

        [HttpGet]
        public IActionResult CreateActor()
        {
            var model = new CreateActorViewModel
            {
                Genders = new List<Gender>
                {
                    Gender.Male,
                    Gender.Female
                }.Select(gender => new SelectListItem { Value = gender.ToString(), Text = gender.ToString() })
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateActor(CreateActorViewModel model)
        {
            var isFailed = true;
            var isImage = false;
            if (ModelState.IsValid)
            {
                var actor = mapper.Map<Actor>(model);
                if (model.Image is null)
                {
                    actor.ImgUrl = _Image.Actor;
                    isImage = true;
                }
                else if (model.Image.ContentType.ToLower().Contains("image"))
                {
                    var newFileName = string.Concat(Guid.NewGuid(), Path.GetExtension(model.Image.FileName));
                    var url = Path.Combine(_Image.ActorImages, newFileName);
                    var path = Path.Combine(webHostEnvironment.WebRootPath, url);
                    using (var fileStream = System.IO.File.Create(path))
                    {
                        model.Image.CopyTo(fileStream);
                    }

                    actor.ImgUrl = url;
                    isImage = true;

                }

                if (isImage&&await ufw.Actors.AddActorAsync(actor))
                {
                    TempData[_TempData.Success] = "Reviewer Created Successfully";
                    isFailed = false;
                }
                else if (isImage&&actor.ImgUrl != _Image.Actor)
                {
                    System.IO.File.Delete(Path.Combine(webHostEnvironment.WebRootPath, actor.ImgUrl));
                }

                if (isFailed)
                {
                    if (!isImage)
                    {
                        TempData[_TempData.Warning] = "please Upload a correct file Image";
                    }
                    TempData[_TempData.Danger] = "Failed To Create A Reviewer";
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
        public async Task<IActionResult> Edit(int actorId,string returnUrl)
        {
            var model = mapper.Map<EditActorViewModel>(await ufw.Actors.GetActorByIdAsync(actorId));

            model.Genders = new List<Gender>
                {
                    Gender.Male,
                    Gender.Female
                }.Select(gender => new SelectListItem { Text = gender.ToString(), Value = gender.ToString() });

            model.ReturnUrl = returnUrl;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditActorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var actor = await ufw.Actors.GetActorByIdAsync(model.Id);
                var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, actor.ImgUrl);
                var oldImageUrl = actor.ImgUrl;
                string newImagePath = null;
                actor = mapper.Map<Actor>(model);

                if (model.Image is null||model.Image.ContentType.ToLower().Contains("image"))
                {
                    if(model.Image is not null)
                    {
                        var newImageName = String.Concat(Guid.NewGuid(), Path.GetExtension(model.Image.FileName));
                        var imageUrl = Path.Combine(_Image.ActorImages, newImageName);
                        newImagePath = Path.Combine(webHostEnvironment.WebRootPath, imageUrl);
                        using (var fileStream = System.IO.File.Create(newImagePath))
                        {
                            model.Image.CopyTo(fileStream);
                        }
                        actor.ImgUrl = imageUrl;
                    }
                    else
                    {
                        actor.ImgUrl = oldImageUrl;
                    }

                    if(await ufw.Actors.UpdateActorAsync(actor))
                    {
                        if(model.Image is not null && oldImageUrl != _Image.Actor && System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        TempData[_TempData.Success] = "actor Edited Successfully";
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

            TempData[_TempData.Danger] = "Failed To Edit The Actor";

            model.Genders = new List<Gender>
                {
                    Gender.Male,
                    Gender.Female
                }.Select(gender => new SelectListItem { Text = gender.ToString(), Value = gender.ToString() });

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveActor(int id,string returnUrl)
        {
            var actor = await ufw.Actors.GetActorByIdAsync(id);
            if (actor is not null)
            {
                if (await ufw.Actors.RemoveActorById(id))
                {
                    var oldPath = Path.Combine(webHostEnvironment.WebRootPath, actor.ImgUrl);
                    if (actor.ImgUrl!=_Image.Actor&&System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                    TempData[_TempData.Success] = "Actor Removed Successfully";
                }
            }
            else
            {
                TempData[_TempData.Danger] = "Falied To Remove An Actor";
            }

            return Redirect(returnUrl);

        }

        [HttpPost,HttpGet]
        public async Task<IActionResult> Search(int page = 1,string searchInput=null)
        {
            var actors = await ufw.Actors.GetAllActorsByNameAsync((page - 1) * _Pagination.PageSize, _Pagination.PageSize,searchInput);

            var pages = Math.Ceiling(await ufw.Actors.CountActorsByNameAsync(searchInput) / (double)_Pagination.PageSize);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;
            ViewBag.SearchInput = searchInput;

            return View(nameof(Index),mapper.Map<IEnumerable<ActorsIndexViewModel>>(actors));
        }

        [HttpGet]
        public async Task<IActionResult> Details(int actorId)
        {
            var actor = await ufw.Actors.GetActorByIdAsync(actorId);
            return View(mapper.Map<DetailsActorViewModel>(actor));
        }

    }
}
