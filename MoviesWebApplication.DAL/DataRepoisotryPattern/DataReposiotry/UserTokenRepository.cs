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
    public class UserTokenRepository:QueryManager,IUserTokenRepository
    {
        private readonly string connectionString;
        public UserTokenRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }


        public async Task<UserToken> GetUserTokenAsync(User user, string Token)
        {
            var statement = @"select UserId,Token,ValidTill from UserTokens where UserId=@par1 and Token=@par2";

            var paramtersDefinition = @"@par1 int,@par2 nvarchar(max)";

            var paramtersValues = @$"@par1 ='{user.Id}',@par2 ='{Token}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            UserToken userToken = null;
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
                            userToken = new UserToken
                            {
                                UserId = Convert.ToInt32(reader[0]),
                                Token = reader[1].ToString(),
                                ValidTill = Convert.ToDateTime(reader[2])
                            };
                        }
                    }

                    if (userToken is not null && DateTime.Now <= userToken.ValidTill)
                    {
                        var result = await RemoveUserTokenAsync(user, Token);
                        if (result)
                        {
                            return null;
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

            return userToken;
        }

        public async Task<bool> RemoveUserTokenAsync(User user, string Token)
        {
            var statement = @"delete from UserTokens where UserId=@par1 and Token=@par2";

            var paramtersDefinition = @"@par1 int,@par2 nvarchar(max)";

            var paramtersValues = @$"@par1 ='{user.Id}',@par2 ='{Token}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;

        }

        public async Task<bool> AddUserTokenAsync(User user, string Token, int Hours = 1)
        {
            var statement = @"insert into UserTokens(UserId,Token,ValidTill) values(@par1,@par2,@par3)";

            var paramtersDefinition = @"@par1 int,@par2 nvarchar(max),@par3 DateTime2";

            var paramtersValues = @$"@par1 ='{user.Id}',@par2 ='{Token}',@par3 ='{DateTime.Now.AddHours(Hours)}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }


    }
}
