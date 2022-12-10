using MoviesWebApplication.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataRepoisotryPattern.IDataRepository
{
    public interface IUserTokenRepository
    {
        Task<UserToken> GetUserTokenAsync(User user, string Token);
        Task<bool> RemoveUserTokenAsync(User user, string Token);
        Task<bool> AddUserTokenAsync(User user, string Token, int Hours = 1);
    }
}
