using MoviesWebApplication.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataRepoisotryPattern.IDataRepository
{
    public interface IMoviesDirectorsRepository
    {
        Task<bool> AddMovieDirectorAsync(MovieDirector movieDirector);
        Task<bool> UpdateMovieDirectorAsync(MovieDirector movieDirector);
        Task<bool> RemoveMovieDirectorAsync(MovieDirector movieDirector);
        Task<MovieDirector> GetMovieDirectorAsync(int movieId, int directorId);
    }
}
