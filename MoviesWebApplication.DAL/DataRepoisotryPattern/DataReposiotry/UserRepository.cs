using Microsoft.Extensions.Options;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.DLL.IDataRepository;
using MoviesWebApplication.Web.DALOptions;
using System.Data.SqlClient;

namespace MoviesWebApplication.DAL.DataReposiotry
{
    public class UserRepository : QueryManager, IUserRepository
    {
        private readonly string connectionString;
        public UserRepository(string conncetionString)
        {
            this.connectionString = conncetionString;
        }

        public async Task<User> AddUserAsync(User user)
        {
            var statement = @"insert into Users(FirstName,LastName,UserName,Email,EmailConfirmed,PasswordHash,IsBlocked,ImgUrl)
                              values(@par1, @par2, @par3, @par4, @par5, @par6, @par7,@par8)";

            var paramtersDefinition = @"@par1 nvarchar(20) , @par2 nvarchar(20),@par3 nvarchar(256), @par4 nvarchar(256) ,
                                          @par5 bit, @par6 varchar(max) , @par7 bit,@par8 varchar(450)";

            var paramtersValues = @$"@par1 = '{user.FirstName}',@par2 = '{user.LastName}',@par3 = '{user.Email}',
                                     @par4 = '{user.Email}',@par5 = {Convert.ToInt32(user.EmailConfirmed)},@par6 = '{user.PasswordHash}',
                                     @par7 = {user.IsBlocked},@par8 = '{user.ImgUrl}'";

            var rowsAffected = await ExecuteQueryAsync(GenerateSql(statement, paramtersDefinition, paramtersValues), connectionString);

            if (rowsAffected > 0)
            {
                return await GetUserByUserNameAsync(user.UserName);
            }

            return null;

        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var statement = @"update users set FirstName=@par2,
                              LastName=@par3,UserName=@par4,Email=@par5,EmailConfirmed=@par6,
                              PasswordHash=@par7,IsBlocked=@par8,ImgUrl=@par9 where Id=@par1";

            var paramtersDefinition = @"@par1 int,@par2 nvarchar(20) , @par3 nvarchar(20)
                                       ,@par4 nvarchar(256), @par5 nvarchar(256) , @par6 bit, @par7 varchar(max) , @par8 bit,@par9 varchar(450)";

            var paramtersValues = @$"@par1='{user.Id}',@par2 = '{user.FirstName}',@par3 = '{user.LastName}',@par4 = '{user.UserName}',
									 @par5 = '{user.Email}',@par6 = {user.EmailConfirmed},@par7 = '{user.PasswordHash}',@par8 = {user.IsBlocked},@par9 = '{user.ImgUrl}'";

            var rowsAffected = await ExecuteQueryAsync(GenerateSql(statement, paramtersDefinition, paramtersValues), connectionString);

            return rowsAffected > 0;
        }

        public async Task<User> GetUserByIdAsync(int Id)
        {
            var statement = @"select Id,FirstName,LastName,UserName,Email,EmailConfirmed,PasswordHash,IsBlocked,ImgUrl from Users where Id=@par1";

            var paramtersDefinition = @"@par1 int";

            var paramtersValues = @$"@par1 ='{Id}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            User user = null;
            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand(sql, cn);
                    await cn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new User
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                UserName = reader[3].ToString(),
                                Email = reader[4].ToString(),
                                EmailConfirmed = Convert.ToBoolean(reader[5]),
                                PasswordHash = reader[6].ToString(),
                                IsBlocked = Convert.ToBoolean((reader[7])),
                                ImgUrl = reader[8].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //....
            }
            finally
            {
                //.....
            }

            return user;

        }

        public async Task<User> GetUserByIdIncludesRoleAsync(int Id)
        {
            var statement = @"select U.Id,U.FirstName,U.LastName,U.UserName,U.Email,U.EmailConfirmed,U.PasswordHash,U.IsBlocked,U.ImgUrl,R.Id,R.Name from Users as U
                              left join UserRoles as UR on U.Id=UR.UserId 
                              left join Roles as R on R.Id=UR.RoleId
                              where U.Id = @par1";
            var paramtersDefinition = @"@par1 int";

            var paramtersValues = @$"@par1 ='{Id}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            User user = null;
            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand(sql, cn);
                    await cn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new User
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                UserName = reader[3].ToString(),
                                Email = reader[4].ToString(),
                                EmailConfirmed = Convert.ToBoolean(reader[5]),
                                PasswordHash = reader[6].ToString(),
                                IsBlocked = Convert.ToBoolean((reader[7])),
                                ImgUrl = reader[8].ToString(),
                                Role = new Role
                                {
                                    Id = Convert.ToInt32(reader[9] != DBNull.Value ? reader[9]:null),
                                    Name = reader[10].ToString()
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //....
            }
            finally
            {
                //.....
            }

            return user;

        }

        public async Task<User> GetUserByUserNameIncludesRoleAsync(string UserName)
        {
            var statement = @"select U.Id,U.FirstName,U.LastName,U.UserName,U.Email,U.EmailConfirmed,U.PasswordHash,U.IsBlocked,U.ImgUrl,R.Id,R.Name from Users as U
                              left join UserRoles as UR on U.Id=UR.UserId 
                              left join Roles as R on R.Id=UR.RoleId
                              where U.UserName = @par1";
            var paramtersDefinition = @"@par1 nvarchar(256)";

            var paramtersValues = @$"@par1 ='{UserName}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            User user = null;
            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand(sql, cn);
                    await cn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new User
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                UserName = reader[3].ToString(),
                                Email = reader[4].ToString(),
                                EmailConfirmed = Convert.ToBoolean(reader[5]),
                                PasswordHash = reader[6].ToString(),
                                IsBlocked = Convert.ToBoolean((reader[7])),
                                ImgUrl = reader[8].ToString(),
                                Role = new Role
                                {
                                    Id = Convert.ToInt32(reader[9] != DBNull.Value ? reader[9] : null),
                                    Name = reader[10].ToString()
                                }
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //....
            }
            finally
            {
                //.....
            }

            return user;

        }

        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            var statement = @"select Id,FirstName,LastName,UserName,Email,EmailConfirmed,PasswordHash,IsBlocked,ImgUrl from Users where UserName=@par1";

            var paramtersDefinition = @"@par1 nvarchar(256)";

            var paramtersValues = @$"@par1 ='{userName}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            User user = null;
            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand(sql, cn);
                    await cn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            user = new User
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                UserName = reader[3].ToString(),
                                Email = reader[4].ToString(),
                                EmailConfirmed = Convert.ToBoolean(reader[5]),
                                PasswordHash = reader[6].ToString(),
                                IsBlocked = Convert.ToBoolean((reader[7])),
                                ImgUrl = reader[8].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //....
            }
            finally
            {
                //.....
            }

            return user;

        }

        public async Task<bool> RemoveUserAsync(User user)
        {
            var statement = @"delete from users where Id=@par1";

            var paramtersDefinition = @"@par1 int";

            var paramtersValues = @$"@par1 ='{user.Id}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;

        }

        public async Task<bool> CheckIfUserEmailExists(string Email)
        {
            var statement = @"select Email from users where Email = @par1";

            var paramtersDefinition = @"@par1 nvarchar(256)";

            var paramtersValues = @$"@par1 ='{Email}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            bool isThereAnEmail = false;

            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand(sql, cn);
                    await cn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        isThereAnEmail = await reader.ReadAsync();
                    }
                }
            }
            catch
            {
                //...
            }
            finally
            {
                //...
            }

            return isThereAnEmail;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            var sql = "select Id,FirstName,LastName,UserName,Email,EmailConfirmed,PasswordHash,IsBlocked,ImgUrl from users";

            var users = new List<User>();

            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand(sql, cn);
                    await cn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                UserName = reader[3].ToString(),
                                Email = reader[4].ToString(),
                                EmailConfirmed = Convert.ToBoolean(reader[5]),
                                PasswordHash = reader[6].ToString(),
                                IsBlocked = Convert.ToBoolean(reader[7]),
                                ImgUrl = reader[8].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
                //....
            }
            finally
            {
                //....
            }

            return users;

        }

        public async Task<IEnumerable<User>> GetAllUsersAsync(int skip ,int take)
        {
            var statement = @"select Id,FirstName,LastName,UserName,Email,EmailConfirmed,PasswordHash,IsBlocked,ImgUrl from users
                        order by Id offset @par1 rows fetch next @par2 rows only";

            var paramtersDefinition = @"@par1 int,@par2 int";

            var paramtersValues = @$"@par1 ={skip},@par2={take}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var users = new List<User>();

            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand(sql, cn);
                    await cn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                UserName = reader[3].ToString(),
                                Email = reader[4].ToString(),
                                EmailConfirmed = Convert.ToBoolean(reader[5]),
                                PasswordHash = reader[6].ToString(),
                                IsBlocked = Convert.ToBoolean(reader[7]),
                                ImgUrl = reader[8].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
                //....
            }
            finally
            {
                //....
            }

            return users;

        }

        public async Task<IEnumerable<User>> GetAllUsersIncludesRoleAsync(int skip, int take)
        {
            var statement = @"select U.Id,U.FirstName,U.LastName,U.UserName,U.Email,U.EmailConfirmed,U.PasswordHash,U.IsBlocked,U.ImgUrl,R.Id,R.Name from Users as U
                         left join UserRoles as UR on U.Id=UR.UserId 
                         left join Roles as R on R.Id=UR.RoleId
                         order by U.Id 
                         offset @par1 rows 
                         fetch next @par2 rows only";

            var paramtersDefinition = @"@par1 int,@par2 int";

            var paramtersValues = @$"@par1 ={skip},@par2={take}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var users = new List<User>();

            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand(sql, cn);
                    await cn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                UserName = reader[3].ToString(),
                                Email = reader[4].ToString(),
                                EmailConfirmed = Convert.ToBoolean(reader[5]),
                                PasswordHash = reader[6].ToString(),
                                IsBlocked = Convert.ToBoolean((reader[7])),
                                ImgUrl = reader[8].ToString(),
                                Role = new Role
                                {
                                    Id = Convert.ToInt32(reader[9] != DBNull.Value ? reader[9] : null),
                                    Name = reader[10].ToString()
                                }
                            });

                        }
                    }

                }
            }
            catch
            {
                //...
            }
            finally
            {
                //...
            }

            return users;

        }

        public async Task<bool> AddUserRoleAsync(User user,Role role)
        {
            var statement = @"insert into UserRoles(UserId, RoleId) values(@par1, @par2)";

            var paramtersDefinition = @"@par1 int,@par2 int";

            var paramtersValues = @$"@par1 ={user.Id},@par2={role.Id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;

        }

        public async Task<bool> ChangeUserRoleAsync(User user, Role role)
        {
            var statement = @"update UserRoles set RoleId=@par2 where UserId=@par1";

            var paramtersDefinition = @"@par1 int,@par2 int";

            var paramtersValues = @$"@par1 ={user.Id},@par2={role.Id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<int> CountAllUsersAsync()
        {
            var sql = @"select count(*) from users";

            using(var cn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand(sql, cn);
                await cn.OpenAsync();
                using(var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        return Convert.ToInt32(reader[0]);
                }
            }

            return -1;

        }

        public async Task<int> CountUsersAsync(bool blocked=false)
        {
            var sql = @$"select count(*) from users where isBlocked = {Convert.ToInt32(blocked)}";

            using (var cn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand(sql, cn);
                await cn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        return Convert.ToInt32(reader[0]);
                }
            }

            return -1;

        }

        public async Task<IEnumerable<User>> GetUsersIncludesRoleAsync(int skip, int take,bool blocked)
        {
            var statement = @"select U.Id,U.FirstName,U.LastName,U.UserName,U.Email,U.EmailConfirmed,U.PasswordHash,U.IsBlocked,U.ImgUrl,R.Id,R.Name from Users as U
                         left join UserRoles as UR on U.Id=UR.UserId 
                         left join Roles as R on R.Id=UR.RoleId
                         where isBlocked=@par3
                         order by U.Id 
                         offset @par1 rows 
                         fetch next @par2 rows only";

            var paramtersDefinition = @"@par1 int,@par2 int,@par3 bit";

            var paramtersValues = @$"@par1 ={skip},@par2={take},@par3={Convert.ToInt32(blocked)}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var users = new List<User>();

            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand(sql, cn);
                    await cn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                UserName = reader[3].ToString(),
                                Email = reader[4].ToString(),
                                EmailConfirmed = Convert.ToBoolean(reader[5]),
                                PasswordHash = reader[6].ToString(),
                                IsBlocked = Convert.ToBoolean((reader[7])),
                                ImgUrl = reader[8].ToString(),
                                Role = new Role
                                {
                                    Id = Convert.ToInt32(reader[9] != DBNull.Value ? reader[9] : null),
                                    Name = reader[10].ToString()
                                }
                            });

                        }
                    }

                }
            }
            catch
            {
                //...
            }
            finally
            {
                //...
            }

            return users;

        }

        public async Task<IEnumerable<User>> FindUsersByEmailIncludesRoleAsync(int skip, int take, string Email)
        {
            var statement = @"select U.Id,U.FirstName,U.LastName,U.UserName,U.Email,U.EmailConfirmed,U.PasswordHash,U.IsBlocked,U.ImgUrl,R.Id,R.Name from Users as U
                         left join UserRoles as UR on U.Id=UR.UserId 
                         left join Roles as R on R.Id=UR.RoleId
                         where U.Email like @par3
                         order by U.Id 
                         offset @par1 rows 
                         fetch next @par2 rows only";

            var paramtersDefinition = @"@par1 int,@par2 int,@par3 nvarchar(256)";

            var paramtersValues = @$"@par1 ={skip},@par2={take},@par3='%{Email}%'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var users = new List<User>();

            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand(sql, cn);
                    await cn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                UserName = reader[3].ToString(),
                                Email = reader[4].ToString(),
                                EmailConfirmed = Convert.ToBoolean(reader[5]),
                                PasswordHash = reader[6].ToString(),
                                IsBlocked = Convert.ToBoolean((reader[7])),
                                ImgUrl = reader[8].ToString(),
                                Role = new Role
                                {
                                    Id = Convert.ToInt32(reader[9] != DBNull.Value ? reader[9] : null),
                                    Name = reader[10].ToString()
                                }
                            });

                        }
                    }

                }
            }
            catch
            {
                //...
            }
            finally
            {
                //...
            }

            return users;
        }

        public async Task<int> CountUsersWhereEmailAsync(string Email)
        {
            var statement = @"select count(*) from users where Email like @par1";

            var paramtersDefinition = @"@par1 nvarchar(256)";

            var paramtersValues = @$"@par1='%{Email}%'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            using (var cn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand(sql, cn);
                await cn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        return Convert.ToInt32(reader[0]);
                }
            }

            return -1;
        }

    }

}
