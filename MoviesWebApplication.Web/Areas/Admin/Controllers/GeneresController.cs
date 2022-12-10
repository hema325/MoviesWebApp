using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.DAL.IDataRepository;
using MoviesWebApplication.Web.Areas.Admin.Models.GeneresModels;
using MoviesWebApplication.Web.Constrains;

namespace MoviesWebApplication.Web.Areas.Admin.Controllers
{
    public class GeneresController : AdminBaseController
    {
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IUnitOfWork ufw;
        private readonly IMapper mapper;
        public GeneresController(IWebHostEnvironment webHostEnvironment,IUnitOfWork ufw,IMapper mapper)
        {
            this.webHostEnvironment = webHostEnvironment;
            this.ufw = ufw;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var Generes = await ufw.Generes.GetAllGeneresAsync((page - 1) * _Pagination.PageSize, _Pagination.PageSize);

            var pages = Math.Ceiling(await ufw.Generes.CountGeneresAsync() / (double)_Pagination.PageSize);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;

            return View(mapper.Map<IEnumerable<GeneresIndexViewModel>>(Generes));
        }

        [HttpGet]
        public async Task<IActionResult> RemoveGenere(int id,string returnUrl)
        {
            var genere = await ufw.Generes.GetGenereByIdAsync(id);
            if(genere is not null)
            {
                await ufw.Generes.RemoveGenereAsync(genere);
                TempData[_TempData.Success] = "Genere Removed Successfully";
            }
            else
            {
                TempData[_TempData.Danger] = "Failed To Remove Genere";
            }

            return Redirect(returnUrl);

        }

        [HttpGet]
        public IActionResult CreateGenere()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGenere(CreateGenereViewModel model)
        {
            if (ModelState.IsValid&& await ufw.Generes.AddGenereAsync(mapper.Map<Genere>(model)))
            {
                TempData[_TempData.Success] = "Genere Added Successfully";
            }
            else
            {
                TempData[_TempData.Danger] = "Failed To Add Genere";
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int genereId,string returnUrl)
        {
            var model = mapper.Map<EditGenereViewModel>(await ufw.Generes.GetGenereByIdAsync(genereId));
            model.Id = genereId;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditGenereViewModel model)
        {
            if (ModelState.IsValid)
            {
                var genere = mapper.Map<Genere>(model);
                if(await ufw.Generes.UpdateGenereAsync(genere))
                {
                    TempData[_TempData.Success] = "Genere Edited Successfully";
                    return Redirect(model.ReturnUrl);
                }
            }

            TempData[_TempData.Danger] = "Failed To Edit Genere";

            return View(model);
        }

        [HttpGet,HttpPost]
        public async Task<IActionResult> Search(string searchInput,int page = 1)
        {
            var Generes = await ufw.Generes.GetAllGeneresByNameAsync((page - 1) * _Pagination.PageSize, _Pagination.PageSize,searchInput);

            var pages = Math.Ceiling(await ufw.Generes.CountGeneresByNameAsync(searchInput) / (double)_Pagination.PageSize);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;
            @ViewBag.SearchInput = searchInput;

            return View(nameof(Index),mapper.Map<IEnumerable<GeneresIndexViewModel>>(Generes));
        }

    }
}
