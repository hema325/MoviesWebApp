using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.DAL.DataRepoisotryPattern.IDataRepository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataRepoisotryPattern.DataReposiotry
{
    public class RoleRepository:QueryManager,IRoleRepository
    {
        private readonly string connectionString;
        public RoleRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<bool> AddRoleAsync(Role role)
        {
            var statement = @"insert into Roles(Name) values(@par1)";

            var paramtersDefinition = @"@par1 nvarchar(256)";

            var paramtersValues = @$"@par1 ='{role.Name}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<bool> AddRoleAsync(List<Role> roles)
        {
            var stringBuilder = new StringBuilder();
            var sz = roles.Count();

            stringBuilder.Append("insert into Roles(Name) values");
            for (var cnt = 1; cnt <= sz; ++cnt)
            {
                stringBuilder.Append($"(@par{cnt})");
                if (cnt != sz)
                    stringBuilder.Append(",");
            }

            var statement = stringBuilder.ToString();

            stringBuilder.Clear();
            for(var cnt = 1; cnt <= sz; ++cnt)
            {
                stringBuilder.Append($"@par{cnt} nvarchar(256)");
                if (cnt != sz)
                    stringBuilder.Append(",");
            }

            var paramtersDefinition = stringBuilder.ToString();

            stringBuilder.Clear();
            for(var cnt = 0; cnt < sz; ++cnt)
            {
                stringBuilder.Append($"@par{cnt + 1}='{roles[cnt].Name}'");
                if (cnt != sz - 1)
                    stringBuilder.Append(",");
            }

            var paramtersValues = stringBuilder.ToString();

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;

        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync()
        {
            var sql = @"select Id, Name from Roles";

            var roles = new List<Role>();

            using (var cn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand(sql, cn);
                await cn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while(await reader.ReadAsync())
                    {
                        roles.Add(new Role { Id = Convert.ToInt32(reader[0]),Name = reader[1].ToString() });
                    }
                }
            }

            return roles;

        }

        public async Task<IEnumerable<Role>> GetAllRolesAsync(int skip,int take)
        {
            var statement = @"select Id,Name from Roles order by ID offset @par1 rows fetch next @par2 rows only";

            var paramtersDefinition = @"@par1 int,@par2 int";

            var paramtersValues = @$"@par1 ={skip},@par2={take}";

            var sql = GenerateSql(statement,paramtersDefinition,paramtersValues);

            var roles = new List<Role>();

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
                            roles.Add(new Role { Id = Convert.ToInt32(reader[0]), Name = reader[1].ToString() });
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

            return roles;

        }

        public async Task<bool> RemoveRoleAsync(Role role)
        {
            var statement = @"delete from Roles where Id=@par1";

            var paramtersDefinition = @"@par1 int";

            var paramtersValues = @$"@par1 ='{role.Id}'";

            var sql = GenerateSql(statement,paramtersDefinition,paramtersValues); 

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<Role> GetRoleByNameAsync(string name)
        {
            var statement = @"select Id,Name from Roles where Name = @par1";

            var paramtersDefinition = @"@par1 nvarchar(256)";

            var paramtersValues = @$"@par1 ='{name}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            Role role = null;

            using (var cn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand(sql, cn);
                await cn.OpenAsync();
                using(var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        role = new Role
                        {
                            Id = Convert.ToInt32(reader[0]),
                            Name = reader[1].ToString()
                        };
                    }
                }
            }

            return role;
        }

        public async Task<Role> GetRoleByIdAsync(int Id)
        {
            var statement = @"select Id,Name from Roles where Id = @par1";

            var paramtersDefinition = @"@par1 int";

            var paramtersValues = @$"@par1 ={Id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            Role role = null;

            using (var cn = new SqlConnection(connectionString))
            {
                var cmd = new SqlCommand(sql, cn);
                await cn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        role = new Role
                        {
                            Id = Convert.ToInt32(reader[0]),
                            Name = reader[1].ToString()
                        };
                    }
                }
            }

            return role;
        }

    }
}
