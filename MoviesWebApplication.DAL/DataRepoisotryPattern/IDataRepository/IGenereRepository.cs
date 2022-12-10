using MoviesWebApplication.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataRepoisotryPattern.IDataRepository
{
    public interface IGenereRepository
    {
        Task<bool> AddGenereAsync(Genere genere);
        Task<bool> UpdateGenereAsync(Genere genere);
        Task<Genere> GetGenereByIdAsync(int id);
        Task<IEnumerable<Genere>> GetAllGeneresAsync(int skip,int take);
        Task<IEnumerable<Genere>> GetAllGeneresByNameAsync(int skip, int take, string name);
        Task<int> CountGeneresAsync();
        Task<int> CountGeneresByNameAsync(string name);
        Task<bool> RemoveGenereAsync(Genere genere);
        Task<Genere> GetGenereByNameAsync(string name);
        Task<IEnumerable<Genere>> GetAllGeneresAsync();
        Task<IEnumerable<Genere>> GetAllGeneresForMovieAsync(int movieId);
    }
}
