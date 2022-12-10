using AutoMapper;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.Web.Areas.Admin.Models.DirectorsModels;
using MoviesWebApplication.Web.Areas.Admin.Models.MoviesModels;
using MoviesWebApplication.Web.Models.MoviesModels;

namespace MoviesWebApplication.Web.AutoMapperProfiles
{
    public class DirectorsProfile:Profile
    {
        public DirectorsProfile()
        {
            CreateMap<Director, DirectorsIndexViewModel>().ForMember(model => model.Name, options => options.MapFrom(user => $"{user.FirstName} {user.LastName}"));
            CreateMap<CreateDirectorViewModel, Director>();
            CreateMap<Director, DetailsDirectorViewModel>().ForMember(model => model.Name, options => options.MapFrom(actor => $"{actor.FirstName} {actor.LastName}"));
            CreateMap<EditDirectorViewModel, Director>();
            CreateMap<Director, EditDirectorViewModel>();
            CreateMap<AddMovieDirectorViewModel, MovieDirector>();

            CreateMap<Director, DirectorsIncludeMoviesDirectorsViewModel>().ForMember(model => model.Name, options => options.MapFrom(director => $"{director.FirstName} {director.LastName}"))
                                                                          .ForMember(model => model.MovieId, options => options.MapFrom(director => director.MovieDirector.MovieId))
                                                                          .ForMember(model => model.DirectorId, options => options.MapFrom(director => director.MovieDirector.DirectorId));

            CreateMap<Director,MovieDirectorsViewModel>().ForMember(model => model.Name, options => options.MapFrom(director => $"{director.FirstName} {director.LastName}"))
                                                                          .ForMember(model => model.Role, options => options.MapFrom(director => "director"));

        }
    }
}
