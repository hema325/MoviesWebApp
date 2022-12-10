using MoviesWebApplication.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataRepoisotryPattern.IDataRepository
{
    public interface IMoviesGeneresRepository
    {
        Task<bool> AddMovieGenereAsync(MovieGenere movieGenere);
        Task<bool> UpdateMovieGenereAsync(MovieGenere movieGenere);
        Task<bool> RemoveMovieGenereAsync(MovieGenere movieGenere);
        Task<MovieGenere> GettMovieGenereAsync(int movieId, int genereId);
        Task<bool> AddMoviesGeneresAsync(IEnumerable<MovieGenere> moviesGeneres);
    }
}
