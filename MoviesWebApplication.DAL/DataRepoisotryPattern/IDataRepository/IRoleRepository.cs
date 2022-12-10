using MoviesWebApplication.DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataRepoisotryPattern.IDataRepository
{
    public interface IRoleRepository
    {
        Task<bool> AddRoleAsync(Role role);
        Task<bool> AddRoleAsync(List<Role> roles);
        Task<IEnumerable<Role>> GetAllRolesAsync();
        Task<IEnumerable<Role>> GetAllRolesAsync(int skip, int take);
        Task<bool> RemoveRoleAsync(Role role);
        Task<Role> GetRoleByNameAsync(string name);

        Task<Role> GetRoleByIdAsync(int Id);

    }
}
