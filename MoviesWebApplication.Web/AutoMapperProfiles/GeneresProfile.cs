using AutoMapper;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.Web.Areas.Admin.Models.GeneresModels;
using MoviesWebApplication.Web.Areas.Admin.Models.MoviesModels;

namespace MoviesWebApplication.Web.AutoMapperProfiles
{
    public class GeneresProfile:Profile
    {
        public GeneresProfile()
        {
            CreateMap<Genere, GeneresIndexViewModel>();
            CreateMap<CreateGenereViewModel, Genere>();
            CreateMap<EditGenereViewModel, Genere>();
            CreateMap<Genere, EditGenereViewModel>();
            CreateMap<AddMovieGenereViewModel, MovieGenere>();

            CreateMap<Genere, GeneresIncludeMoviesGeneresViewModel>().ForMember(model => model.GenereId, options => options.MapFrom(genere => genere.MovieGenere.GenereId))
                                                                    .ForMember(model => model.MovieId, options => options.MapFrom(genere => genere.MovieGenere.MovieId));

        }
    }
}
