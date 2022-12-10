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
    public class DirectorRepository : QueryManager, IDirectorRepository
    {
        private readonly string connectionString;
        public DirectorRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public async Task<bool> AddDirectorAsync(Director director)
        {
            var statement = @"insert into Directors(firstName,lastName,Gender,Biography,ImgUrl) values(@par1,@par2,@par3,@par4,@par5)";

            var paramtersDefinition = @"@par1 nvarchar(20),@par2 nvarchar(20),@par3 nvarchar(20),@par4 nvarchar(max),@par5 nvarchar(450)";

            var paramtersValues = @$"@par1='{director.FirstName}',@par2='{director.LastName}',@par3='{director.Gender}',@par4='{director.Biography}',@par5='{director.ImgUrl}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<int> CountDirectorsAsync()
        {
            var sql = @"select count(*) from directors";

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
                //...
            }

            return -1;
        }

        public async Task<int> CountDirectorsByNameAsync(string name)
        {
            var statement = @"select count(*) from directors where firstName like @par1 or lastName like @par1";

            var paramtersDefinition = @"@par1 nvarchar(40)";

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
                //...
            }

            return -1;
        }

        public async Task<IEnumerable<Director>> GetAllDirectorsAsync(int skip, int take)
        {
            var statement = @"select id, firstName, lastName, Gender, Biography, ImgUrl from directors
                              order by Id
                              offset @par1 rows
                              fetch next @par2 rows only";

            var paramtersDefinition = @"@par1 int,@par2 int";

            var paramtersValues = @$"@par1={skip},@par2= {take}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var directors = new List<Director>();

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
                            directors.Add(new Director
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                Gender = (Gender)Enum.Parse(typeof(Gender), reader[3].ToString()),
                                Biography = reader[4].ToString(),
                                ImgUrl = reader[5].ToString()
                            });

                        }
                    }
                }
            }
            catch
            {
                //...
            }
            return directors;
        }

        public async Task<IEnumerable<Director>> GetAllDirectorsAsync()
        {
            var sql = @"select id, firstName, lastName, Gender, Biography, ImgUrl from directors";

            var directors = new List<Director>();

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
                            directors.Add(new Director
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                Gender = (Gender)Enum.Parse(typeof(Gender), reader[3].ToString()),
                                Biography = reader[4].ToString(),
                                ImgUrl = reader[5].ToString()
                            });

                        }
                    }
                }
            }
            catch
            {
                //...
            }
            return directors;
        }

        public async Task<IEnumerable<Director>> GetAllDirectorsByNameAsync(int skip, int take, string name)
        {
            var statement = @"select id, firstName, lastName, Gender, Biography, ImgUrl from directors where firstName like @par3 or lastName like @par3
                              order by Id
                              offset @par1 rows
                              fetch next @par2 rows only";

            var paramtersDefinition = @"@par1 int,@par2 int,@par3 nvarchar(40)";

            var paramtersValues = @$"@par1={skip},@par2= {take},@par3='%{name}%'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var directors = new List<Director>();

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
                            directors.Add(new Director
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                Gender = (Gender)Enum.Parse(typeof(Gender), reader[3].ToString()),
                                Biography = reader[4].ToString(),
                                ImgUrl = reader[5].ToString()
                            });

                        }
                    }
                }
            }
            catch
            {
                //...
            }
            return directors;
        }

        public async Task<Director> GetDirectorByIdAsync(int id)
        {
            var statement = @"select id, firstName, lastName, Gender, Biography, ImgUrl from Directors where id = @par1";

            var paramtersDefinition = @"@par1 int";

            var paramtersValues = @$"@par1={id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            Director director = null;
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
                            director = new Director
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                Gender = (Gender)Enum.Parse(typeof(Gender), reader[3].ToString()),
                                Biography = reader[4].ToString(),
                                ImgUrl = reader[5].ToString()
                            };

                        }
                    }
                }
            }
            catch
            {
                //...
            }
            return director;
        }
        public async Task<IEnumerable<Director>> GetAllDirectorsForMovieAsync(int movieId)
        {
            var sql = $@"select id,firstName,lastName,Gender,Biography,ImgUrl,movieId,directorId from directors as D 
                         join MoviesDirectors as MD on D.Id = MD.directorId  where MD.MovieId={movieId}";

            var directors = new List<Director>();
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
                            directors.Add(new Director
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                Gender = (Gender)Enum.Parse(typeof(Gender), reader[3].ToString()),
                                Biography = reader[4].ToString(),
                                ImgUrl = reader[5].ToString(),
                                MovieDirector = new MovieDirector
                                {
                                    MovieId = Convert.ToInt32(reader[6]),
                                    DirectorId = Convert.ToInt32(reader[7]),
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
            return directors;
        }

        public async Task<bool> RemoveDirectorById(int id)
        {
            var statement = @"delete from directors where id = @par1";

            var paramtersDefinition = @"@par1 int";

            var paramtersValues = @$"@par1={id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateDirectorAsync(Director director)
        {
            var statement = @"update directors set firstName=@par1, lastName=@par2, Gender=@par3, Biography=@par4, ImgUrl=@par5 from directors where Id=@par6";

            var paramtersDefinition = @"@par1 nvarchar(20),@par2 nvarchar(20),@par3 nvarchar(20),@par4 nvarchar(max),@par5 nvarchar(450),@par6 int";

            var paramtersValues = @$"@par1='{director.FirstName}',@par2='{director.LastName}',@par3='{director.Gender}',@par4='{director.Biography}',@par5='{director.ImgUrl}',@par6={director.Id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }
    }
}
