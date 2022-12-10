using MoviesWebApplication.Web.DALOptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL
{
    public abstract class QueryManager
    {
        public string GenerateSql(string statement, string paramtersDefinition, string paramtersValues)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("exec sp_executesql @statement =N'").
                Append(statement)
               .Append("',@params=N'")
               .Append(paramtersDefinition)
               .Append("',")
               .Append(paramtersValues);

            return stringBuilder.ToString();
        }

        public async Task<int> ExecuteQueryAsync(string sql,string connectionString)
        {
            var rowsAffected = -1;

            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand(sql, cn);
                    await cn.OpenAsync();
                    rowsAffected = await cmd.ExecuteNonQueryAsync();
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

            return rowsAffected;
        }

    }
}
