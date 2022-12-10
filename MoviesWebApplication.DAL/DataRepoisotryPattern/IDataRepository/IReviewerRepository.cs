
using MoviesWebApplication.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataRepoisotryPattern.IDataRepository
{
    public interface IReviewerRepository
    {
        Task<bool> AddReviewerAsync(Reviewer reviewer);
        Task<bool> UpdateReviewerAsync(Reviewer reviewer);
        Task<Reviewer> GetReviewerByIdAsync(int id);
        Task<IEnumerable<Reviewer>> GetAllReviewersAsync();
        Task<IEnumerable<Reviewer>> GetAllReviewersAsync(int skip, int take);
        Task<IEnumerable<Reviewer>> GetAllReviewersByNameAsync(int skip, int take, string name);
        Task<int> CountReviewerAsync();
        Task<int> CountReviewerByNameAsync(string name);
        Task<bool> RemoveReviewerById(int id);
        Task<IEnumerable<Reviewer>> GetAllReviewersForMovieAsync(int movieId);
    }
}
