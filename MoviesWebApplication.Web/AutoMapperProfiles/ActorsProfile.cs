using AutoMapper;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.Web.Areas.Admin.Models.ActorsModels;
using MoviesWebApplication.Web.Areas.Admin.Models.MoviesModels;
using MoviesWebApplication.Web.Models.MoviesModels;

namespace MoviesWebApplication.Web.AutoMapperProfiles
{
    public class ActorsProfile:Profile
    {
        public ActorsProfile()
        {
            CreateMap<Actor, ActorsIndexViewModel>().ForMember(model => model.Name, options => options.MapFrom(actor => $"{actor.FirstName} {actor.LastName}"));
            CreateMap<CreateActorViewModel, Actor>();
            CreateMap<Actor,DetailsActorViewModel>().ForMember(model => model.Name, options => options.MapFrom(actor => $"{actor.FirstName} {actor.LastName}"));
            CreateMap<EditActorViewModel, Actor>();
            CreateMap<Actor, EditActorViewModel>();

            CreateMap<Actor, ActorsIncludeMoviesActorsViewModel>().ForMember(model => model.Name, options => options.MapFrom(actor => $"{actor.FirstName} {actor.LastName}"))
                                                                .ForMember(model => model.MovieId, options => options.MapFrom(actor => actor.MovieActor.MovieId))
                                                                 .ForMember(model => model.ActorId, options => options.MapFrom(actor => actor.MovieActor.ActorId))
                                                                 .ForMember(model => model.Role, options => options.MapFrom(actor => actor.MovieActor.Role));

            CreateMap<AddMovieActorViewModel, MovieActor>();
            CreateMap<Actor, MovieActorsViewModel>().ForMember(model => model.Name, options => options.MapFrom(actor => $"{actor.FirstName} {actor.LastName}"))
                                                    .ForMember(model => model.Role, options => options.MapFrom(actor => actor.MovieActor.Role));


        }
    }
}
