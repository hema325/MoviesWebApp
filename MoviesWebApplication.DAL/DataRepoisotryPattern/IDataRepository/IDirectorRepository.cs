using MoviesWebApplication.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataRepoisotryPattern.IDataRepository
{
    public interface IDirectorRepository
    {
        Task<bool> AddDirectorAsync(Director actor);
        Task<bool> UpdateDirectorAsync(Director actor);
        Task<Director> GetDirectorByIdAsync(int id);
        Task<IEnumerable<Director>> GetAllDirectorsAsync(int skip, int take);
        Task<IEnumerable<Director>> GetAllDirectorsAsync();
        Task<IEnumerable<Director>> GetAllDirectorsByNameAsync(int skip, int take, string name);
        Task<int> CountDirectorsAsync();
        Task<int> CountDirectorsByNameAsync(string name);
        Task<IEnumerable<Director>> GetAllDirectorsForMovieAsync(int movieId);
        Task<bool> RemoveDirectorById(int id);
    }
}
