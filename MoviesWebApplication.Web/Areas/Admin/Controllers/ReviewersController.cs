using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.DAL.IDataRepository;
using MoviesWebApplication.Web.Areas.Admin.Models.ReviewersModels;
using MoviesWebApplication.Web.Constrains;

namespace MoviesWebApplication.Web.Areas.Admin.Controllers
{
    public class ReviewersController : AdminBaseController
    {
        private readonly IUnitOfWork ufw;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment webHostEnvironment;
        public ReviewersController(IUnitOfWork ufw, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            this.ufw = ufw;
            this.mapper = mapper;
            this.webHostEnvironment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var reviewers = await ufw.Reviewers.GetAllReviewersAsync((page - 1) * _Pagination.PageSize, _Pagination.PageSize);

            var pages = Math.Ceiling(await ufw.Reviewers.CountReviewerAsync() / (double)_Pagination.PageSize);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;

            return View(mapper.Map<IEnumerable<ReviewersIndexViewModel>>(reviewers));
        }

        [HttpGet]
        public IActionResult CreateReviewer()
        {
            var model = new CreateReviewerViewModel
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
        public async Task<IActionResult> CreateReviewer(CreateReviewerViewModel model)
        {
            var isFailed = true;
            var isImage = false;
            if (ModelState.IsValid)
            {
                var reviewer = mapper.Map<Reviewer>(model);
                if (model.Image is null)
                {
                    reviewer.ImgUrl = _Image.Reviewer;
                    isImage = true;
                }
                else if (model.Image.ContentType.ToLower().Contains("image"))
                {
                    var newFileName = string.Concat(Guid.NewGuid(), Path.GetExtension(model.Image.FileName));
                    var url = Path.Combine(_Image.ReviewerImages, newFileName);
                    var path = Path.Combine(webHostEnvironment.WebRootPath, url);
                    using (var fileStream = System.IO.File.Create(path))
                    {
                        model.Image.CopyTo(fileStream);
                    }

                    reviewer.ImgUrl = url;
                    isImage = true;

                }

                if (isImage&&await ufw.Reviewers.AddReviewerAsync(reviewer))
                {
                    TempData[_TempData.Success] = "Reviewer Added Successfully";
                    isFailed = false;
                }
                else if (isImage && reviewer.ImgUrl != _Image.Reviewer)
                {
                    System.IO.File.Delete(Path.Combine(webHostEnvironment.WebRootPath, reviewer.ImgUrl));
                }

                if (isFailed)
                {
                    if (!isImage)
                    {
                        TempData[_TempData.Warning] = "please Upload a correct file Image";
                    }
                    TempData[_TempData.Danger] = "Failed To Add A Reviewer";
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
        public async Task<IActionResult> Edit(int reviewerId,string returnUrl)
        {
            var model = mapper.Map<EditReviewerViewModel>(await ufw.Reviewers.GetReviewerByIdAsync(reviewerId));

            model.Genders = new List<Gender>
                {
                    Gender.Male,
                    Gender.Female
                }.Select(gender => new SelectListItem { Text = gender.ToString(), Value = gender.ToString() });

            model.ReturnUrl = returnUrl;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(EditReviewerViewModel model)
        {
            if (ModelState.IsValid)
            {
                var reviewer = await ufw.Reviewers.GetReviewerByIdAsync(model.Id);
                var oldImagePath = Path.Combine(webHostEnvironment.WebRootPath, reviewer.ImgUrl);
                var oldImageUrl = reviewer.ImgUrl;
                string newImagePath = null;
                reviewer = mapper.Map<Reviewer>(model);

                if (model.Image is null || model.Image.ContentType.ToLower().Contains("image"))
                {
                    if (model.Image is not null)
                    {
                        var newImageName = String.Concat(Guid.NewGuid(), Path.GetExtension(model.Image.FileName));
                        var imageUrl = Path.Combine(_Image.ReviewerImages, newImageName);
                        newImagePath = Path.Combine(webHostEnvironment.WebRootPath, imageUrl);
                        using (var fileStream = System.IO.File.Create(newImagePath))
                        {
                            model.Image.CopyTo(fileStream);
                        }
                        reviewer.ImgUrl = imageUrl;
                    }
                    else
                    {
                        reviewer.ImgUrl = oldImageUrl;
                    }

                    if (await ufw.Reviewers.UpdateReviewerAsync(reviewer))
                    {
                        if (model.Image is not null && oldImageUrl != _Image.Reviewer && System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                        TempData[_TempData.Success] = "reviewer Edited Successfully";
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

            TempData[_TempData.Danger] = "Failed To Edit The reviewer";

            model.Genders = new List<Gender>
                {
                    Gender.Male,
                    Gender.Female
                }.Select(gender => new SelectListItem { Text = gender.ToString(), Value = gender.ToString() });

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RemoveReviewer(int id, string returnUrl)
        {
            var reviewer = await ufw.Reviewers.GetReviewerByIdAsync(id);
            if (reviewer is not null)
            {
                if (await ufw.Reviewers.RemoveReviewerById(id))
                {
                    var oldPath = Path.Combine(webHostEnvironment.WebRootPath, reviewer.ImgUrl);
                    if (reviewer.ImgUrl!=_Image.Reviewer&&System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                    TempData[_TempData.Success] = "Reviewer Removed Successfully";
                }
            }
            else
            {
                TempData[_TempData.Danger] = "Falied To Remove A Reviewer";
            }

            return Redirect(returnUrl);

        }


        [HttpPost, HttpGet]
        public async Task<IActionResult> Search(int page = 1, string searchInput = null)
        {
            var reviewers = await ufw.Reviewers.GetAllReviewersByNameAsync((page - 1) * _Pagination.PageSize, _Pagination.PageSize, searchInput);

            var pages = Math.Ceiling(await ufw.Reviewers.CountReviewerByNameAsync(searchInput) / (double)_Pagination.PageSize);

            ViewBag.IsLastPage = pages <= page;
            ViewBag.Page = page;
            ViewBag.SearchInput = searchInput;

            return View(nameof(Index), mapper.Map<IEnumerable<ReviewersIndexViewModel>>(reviewers));
        }


        [HttpGet]
        public async Task<IActionResult> Details(int reviewerId)
        {
            var reviewer = await ufw.Reviewers.GetReviewerByIdAsync(reviewerId);
            return View(mapper.Map<DetailsReviewerViewModel>(reviewer));
        }

    }
}
