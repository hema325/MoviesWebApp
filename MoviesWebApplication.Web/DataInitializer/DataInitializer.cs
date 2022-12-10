using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.DAL.IDataRepository;
using MoviesWebApplication.Web.Constrains;
using MoviesWebApplication.Web.WebOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataBaseManagement.DataBaseInitializer
{
    public class DataInitializer:IDataInitializer
    {
        private readonly IUnitOfWork ufw;
        private readonly IPasswordHasher<User> passwordHasher;
        private readonly AccountOption accountOption;
        private readonly IMapper mapper;
        public DataInitializer(IUnitOfWork ufw,
            IPasswordHasher<User> passwordHasher,
            IOptions<AccountOption> options,
            IMapper mapper)
        {
            this.passwordHasher = passwordHasher;
            this.ufw = ufw;
            this.accountOption = options.Value;
            this.mapper = mapper;
        }
        public async Task<DataInitializer> Initialize()
        {
            if (await ufw.Roles.GetRoleByNameAsync(_Role.Admin) is null)
            {
                await ufw.Roles.AddRoleAsync(new Role { Name = _Role.Admin });
                await ufw.Roles.AddRoleAsync(new Role { Name = _Role.User });
            }

            var user = mapper.Map<User>(accountOption);

            user.PasswordHash = passwordHasher.HashPassword(user,accountOption.Password);

            if (await ufw.Users.GetUserByUserNameAsync(user.UserName) is null)
            {
                await ufw.Users.AddUserAsync(user);
                await ufw.Users.AddUserRoleAsync(await ufw.Users.GetUserByUserNameAsync(user.UserName),
                                                 await ufw.Roles.GetRoleByNameAsync(_Role.Admin));
            }

            return this;
        }

    }
}
