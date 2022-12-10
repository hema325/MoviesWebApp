using MoviesWebApplication.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataRepoisotryPattern.IDataRepository
{
    public interface IMoviesActorsRepository
    {
        Task<bool> AddMovieActorAsync(MovieActor movieActor);
        Task<MovieActor> GetMovieActorAsync(int movieId, int actorId);
        Task<bool> RemoveMovieActorAsync(MovieActor movieActor);
        Task<bool> UpdateMovieActorAsync(MovieActor movieActor);
    }
}
