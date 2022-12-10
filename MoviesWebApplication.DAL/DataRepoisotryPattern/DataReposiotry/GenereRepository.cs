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
    public class GenereRepository :QueryManager, IGenereRepository
    {
        private readonly string connectionString;
        public GenereRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public async Task<bool> AddGenereAsync(Genere genere)
        {
            var statement = @"insert into generes(Name) values(@par1)";

            var paramtersDefinition = @"@par1 nvarchar(100)";

            var paramtersValues = @$"@par1='{genere.Name}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }
        public async Task<int> CountGeneresAsync()
        {
            var sql = @"select count(*) from Generes";

            try
            {
                using(var cn = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand(sql, cn);
                    await cn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if(await reader.ReadAsync())
                        {
                            return Convert.ToInt32(reader[0]);
                        }
                    }
                }
            }
            catch
            {
                //....
            }

            return -1;

        }

        public async Task<int> CountGeneresByNameAsync(string name)
        {
            var statement = @"select count(*) from generes where name like @par1";

            var paramtersDefinition = @"@par1 nvarchar(100)";

            var paramtersValues = @$"@par1='%{name}%'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

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
                            return Convert.ToInt32(reader[0]);
                        }
                    }
                }
            }
            catch
            {
                //....
            }

            return -1;

        }

        public async Task<IEnumerable<Genere>> GetAllGeneresAsync(int skip, int take)
        {
            var statement = @"select id,name from generes
                              order by id 
                              offset @par1 rows 
                              fetch next @par2 rows only";

            var paramtersDefinition = @"@par1 int,@par2 int";

            var paramtersValues = @$"@par1={skip},@par2={take}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var generes = new List<Genere>();
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
                            generes.Add(new Genere
                            {
                                Id = Convert.ToInt32(reader[0]),
                                Name = reader[1].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
                //....
            }

            return generes;
        }

        public async Task<IEnumerable<Genere>> GetAllGeneresAsync()
        {
            var sql = @"select id,name from generes";

            var generes = new List<Genere>();
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
                            generes.Add(new Genere
                            {
                                Id = Convert.ToInt32(reader[0]),
                                Name = reader[1].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
                //....
            }

            return generes;
        }

        public async Task<IEnumerable<Genere>> GetAllGeneresByNameAsync(int skip, int take, string name)
        {
            var statement = @"select id,name from generes where name like @par3
                              order by id 
                              offset @par1 rows 
                              fetch next @par2 rows only";

            var paramtersDefinition = @"@par1 int,@par2 int,@par3 nvarchar(100)";

            var paramtersValues = @$"@par1={skip},@par2={take},@par3='%{name}%'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var generes = new List<Genere>();
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
                            generes.Add(new Genere
                            {
                                Id = Convert.ToInt32(reader[0]),
                                Name = reader[1].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
                //....
            }

            return generes;
        }

        public async Task<Genere> GetGenereByIdAsync(int id)
        {
            var statement = @"select id,name from generes where id = @par1";

            var paramtersDefinition = @"@par1 int";

            var paramtersValues = @$"@par1={id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            Genere genere = null;
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
                            genere = new Genere
                            {
                                Id = Convert.ToInt32(reader[0]),
                                Name = reader[1].ToString()
                            };
                        }
                    }
                }
            }
            catch
            {
                //....
            }

            return genere;
        }

        public async Task<Genere> GetGenereByNameAsync(string name)
        {
            var statement = @"select id,name from generes where name = @par1";

            var paramtersDefinition = @"@par1 nvarchar(100)";

            var paramtersValues = @$"@par1={name}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            Genere genere = null;
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
                            genere = new Genere
                            {
                                Id = Convert.ToInt32(reader[0]),
                                Name = reader[1].ToString()
                            };
                        }
                    }
                }
            }
            catch
            {
                //....
            }

            return genere;
        }

        public async Task<bool> RemoveGenereAsync(Genere genere)
        {
            var statement = @"delete from generes where id = @par1";

            var paramtersDefinition = @"@par1 int";

            var paramtersValues = @$"@par1={genere.Id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;

        }

        public async Task<bool> UpdateGenereAsync(Genere genere)
        {

            var statement = @"update generes set Name = @par1 where id = @par2";

            var paramtersDefinition = @"@par1 nvarchar(100),@par2 int";

            var paramtersValues = @$"@par1='{genere.Name}',@par2={genere.Id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Genere>> GetAllGeneresForMovieAsync(int movieId)
        {
            var sql = @$"select id,name,movieId,genereId from generes as G
                               join MoviesGeneres as MG on G.Id = MG.GenereId where MG.MovieId={movieId}";

            var generes = new List<Genere>();
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
                            generes.Add(new Genere
                            {
                                Id = Convert.ToInt32(reader[0]),
                                Name = reader[1].ToString(),
                                MovieGenere = new MovieGenere
                                {
                                    MovieId = Convert.ToInt32(reader[2]),
                                    GenereId = Convert.ToInt32(reader[3])
                                }
                            });
                        }
                    }
                }
            }
            catch
            {
                //....
            }

            return generes;
        }
    }
}
