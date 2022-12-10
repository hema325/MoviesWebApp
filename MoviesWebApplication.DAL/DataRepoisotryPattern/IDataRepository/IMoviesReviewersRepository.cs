using MoviesWebApplication.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataRepoisotryPattern.IDataRepository
{
    public interface IMoviesReviewersRepository
    {
        Task<bool> AddMovieReviewerAsync(MovieReviewer movieReviewer);
        Task<bool> RemoveMovieReviewerAsync(MovieReviewer movieReviewer);
        Task<bool> UpdateMovieReviewerAsync(MovieReviewer movieReviewer);

        Task<MovieReviewer> GetMovieReviewerAsync(int movieId, int reviewerId);
    }
}
