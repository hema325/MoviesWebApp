using AutoMapper;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.Web.Areas.Admin.Models.MoviesModels;
using MoviesWebApplication.Web.Areas.Admin.Models.ReviewersModels;

namespace MoviesWebApplication.Web.AutoMapperProfiles
{
    public class ReviewersProfile:Profile
    {
        public ReviewersProfile()
        {
            CreateMap<Reviewer, ReviewersIndexViewModel>().ForMember(model => model.Name, options => options.MapFrom(actor => $"{actor.FirstName} {actor.LastName}"));
            CreateMap<CreateReviewerViewModel, Reviewer>();
            CreateMap<Reviewer,DetailsReviewerViewModel>().ForMember(model => model.Name, options => options.MapFrom(actor => $"{actor.FirstName} {actor.LastName}"));
            CreateMap<EditReviewerViewModel, Reviewer>();
            CreateMap<Reviewer, EditReviewerViewModel>();

            CreateMap<Reviewer, ReviewersIncludeMoviesReviewersViewModel>().ForMember(model => model.Name, options => options.MapFrom(actor => $"{actor.FirstName} {actor.LastName}"))
                                                              .ForMember(model => model.MovieId, options => options.MapFrom(actor => actor.MovieReviewer.MovieId))
                                                               .ForMember(model => model.ReviewerId, options => options.MapFrom(actor => actor.MovieReviewer.ReviewerId))
                                                               .ForMember(model => model.Stars, options => options.MapFrom(actor => actor.MovieReviewer.Stars));


            CreateMap<AddMovieReviewerViewModel, MovieReviewer>();
        }
    }
}
