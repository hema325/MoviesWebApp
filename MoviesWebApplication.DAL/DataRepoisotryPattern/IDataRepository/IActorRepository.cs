using MoviesWebApplication.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataRepoisotryPattern.IDataRepository
{
    public interface IActorRepository
    {
        Task<bool> AddActorAsync(Actor actor);
        Task<bool> UpdateActorAsync(Actor actor);
        Task<Actor> GetActorByIdAsync(int id);
        Task<IEnumerable<Actor>> GetAllActorsAsync(int skip, int take);
        Task<IEnumerable<Actor>> GetAllActorsByNameAsync(int skip, int take, string name);
        Task<int> CountActorsAsync();
        Task<int> CountActorsByNameAsync(string name);
        Task<bool> RemoveActorById(int id);
        Task<IEnumerable<Actor>> GetAllActorsAsync();
        Task<IEnumerable<Actor>> GetAllActorsForMovieAsync(int movieId);

    }
}
