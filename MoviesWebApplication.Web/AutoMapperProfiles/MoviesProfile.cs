using AutoMapper;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.Web.Areas.Admin.Models.MoviesModels;
using MoviesWebApplication.Web.Models.MoviesModels;

namespace MoviesWebApplication.Web.AutoMapperProfiles
{
    public class MoviesProfile:Profile
    {
        public MoviesProfile()
        {
            CreateMap<Movie, AdminMoviesIndexViewModel>().ForMember(model=>model.Duration,options=>options.MapFrom(movie=>TimeSpan.FromSeconds(movie.Duration)));
            CreateMap<Movie, DetailsMovieViewModel>().ForMember(model=>model.Duration,options=>options.MapFrom(movie=>TimeSpan.FromSeconds(movie.Duration)));
            CreateMap<CreateMovieViewModel, Movie>().ForMember(movie => movie.Duration, options => options.MapFrom(model => (int)(model.Duration*60*60)));
            CreateMap<Movie, EditMovieViewModel>().ForMember(model=>model.Duration,options=>options.MapFrom(movie=>(double)movie.Duration/(60*60)));
            CreateMap<EditMovieViewModel,Movie>().ForMember(movie => movie.Duration, options => options.MapFrom(model => (int)(model.Duration * 60 * 60)));

            CreateMap<Movie, MoviesIndexViewModel>();
            CreateMap<Movie, WatchMovieViewModel>();
            

            

           
          
        }
    }
}
