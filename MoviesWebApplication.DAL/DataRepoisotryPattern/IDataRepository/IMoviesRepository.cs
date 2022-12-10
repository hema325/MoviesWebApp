using MoviesWebApplication.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataRepoisotryPattern.IDataRepository
{
    public interface IMoviesRepository
    {
        Task<bool> AddMovieAsync(Movie movie);
        Task<bool> UpdateMovieAsync(Movie movie);
        Task<Movie> GetMovieByIdAsync(int id);
        Task<IEnumerable<Movie>> GetAllMoviesAsync(int skip, int take);
        Task<IEnumerable<Movie>> GetAllMoviesByTitleAsync(int skip, int take, string title);
        Task<Movie> GetMovieByTitleAsync(string title);
        Task<int> CountMoviesAsync();
        Task<int> CountMoviesByTitleAsync(string title);
        Task<bool> RemoveMovieById(int id);
        Task<IEnumerable<Movie>> GetAllMoviesHeighstRatedAsync(int skip, int take);


    }
}
