using MoviesWebApplication.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DLL.IDataRepository
{
    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<User> GetUserByUserNameAsync(string userName);
        Task<User> GetUserByIdAsync(int Id);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> RemoveUserAsync(User user);
        Task<bool> CheckIfUserEmailExists(string Email);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetAllUsersAsync(int skip,int take);
        Task<User> GetUserByIdIncludesRoleAsync(int Id);
        Task<IEnumerable<User>> GetAllUsersIncludesRoleAsync(int skip, int take);
        Task<bool> AddUserRoleAsync(User user, Role role);
        Task<User> GetUserByUserNameIncludesRoleAsync(string UserName);
        Task<bool> ChangeUserRoleAsync(User user, Role role);
        Task<int> CountUsersAsync(bool blocked);
        Task<int> CountAllUsersAsync();
        Task<IEnumerable<User>> GetUsersIncludesRoleAsync(int skip, int take,bool blocked);
        Task<int> CountUsersWhereEmailAsync(string Email);
        Task<IEnumerable<User>> FindUsersByEmailIncludesRoleAsync(int skip, int take, string Email);
    }
}
