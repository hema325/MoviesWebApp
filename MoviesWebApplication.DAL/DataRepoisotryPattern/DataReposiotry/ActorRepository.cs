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
    public class ActorRepository : QueryManager, IActorRepository
    {
        private readonly string connectionString;
        public ActorRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public async Task<bool> AddActorAsync(Actor actor)
        {
            var statement = @"insert into actors(firstName,lastName,Gender,Biography,ImgUrl) values(@par1,@par2,@par3,@par4,@par5)";

            var paramtersDefinition = @"@par1 nvarchar(20),@par2 nvarchar(20),@par3 nvarchar(20),@par4 nvarchar(max),@par5 nvarchar(450)";

            var paramtersValues = @$"@par1='{actor.FirstName}',@par2='{actor.LastName}',@par3='{actor.Gender}',@par4='{actor.Biography}',@par5='{actor.ImgUrl}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;

        }

        public async Task<int> CountActorsAsync()
        {
            var sql = @"select count(*) from actors";

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

        public async Task<int> CountActorsByNameAsync(string name)
        {
            var statement = @"select count(*) from actors where firstName like @par1 or lastName like @par1";

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

        public async Task<Actor> GetActorByIdAsync(int id)
        {
            var statement = @"select id, firstName, lastName, Gender, Biography, ImgUrl from actors where id = @par1";

            var paramtersDefinition = @"@par1 int";

            var paramtersValues = @$"@par1={id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            Actor actor = null;
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
                            actor = new Actor
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
            return actor;

        }

        public async Task<IEnumerable<Actor>> GetAllActorsAsync()
        {
            var sql = @"select id, firstName, lastName, Gender, Biography, ImgUrl from actors ";

            var actors = new List<Actor>();

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
                            actors.Add(new Actor
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
            return actors;

        }

        public async Task<IEnumerable<Actor>> GetAllActorsAsync(int skip,int take)
        {
            var statement = @"select id, firstName, lastName, Gender, Biography, ImgUrl from actors
                              order by Id
                              offset @par1 rows
                              fetch next @par2 rows only";

            var paramtersDefinition = @"@par1 int,@par2 int";

            var paramtersValues = @$"@par1={skip},@par2= {take}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var actors = new List<Actor>();

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
                            actors.Add(new Actor
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
            return actors;

        }

        public async Task<IEnumerable<Actor>> GetAllActorsByNameAsync(int skip,int take,string name)
        {
            var statement = @"select id, firstName, lastName, Gender, Biography, ImgUrl from actors where firstName like @par3 or lastName like @par3
                              order by Id
                              offset @par1 rows
                              fetch next @par2 rows only";

            var paramtersDefinition = @"@par1 int,@par2 int,@par3 nvarchar(40)";

            var paramtersValues = @$"@par1={skip},@par2= {take},@par3='%{name}%'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var actors = new List<Actor>();

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
                            actors.Add(new Actor
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
            return actors;
        }

        public async Task<IEnumerable<Actor>> GetAllActorsForMovieAsync(int movieId)
        {
            var sql = @$"select id,firstName,lastName,Gender,Biography,ImgUrl,movieId,actorId,Role from actors as ac 
                         join MoviesActors as mvac on ac.Id = mvac.ActorId where mvac.MovieId={movieId}";

            var actors = new List<Actor>();

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
                            actors.Add(new Actor
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                Gender = (Gender)Enum.Parse(typeof(Gender), reader[3].ToString()),
                                Biography = reader[4].ToString(),
                                ImgUrl = reader[5].ToString(),
                                MovieActor = new MovieActor
                                {
                                    MovieId = Convert.ToInt32(reader[6]),
                                    ActorId = Convert.ToInt32(reader[7]),
                                    Role = reader[8].ToString()
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
            return actors;

        }

        public async Task<bool> RemoveActorById(int id)
        {
            var statement = @"delete from actors where id = @par1";

            var paramtersDefinition = @"@par1 int";

            var paramtersValues = @$"@par1={id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;

        }

        public async Task<bool> UpdateActorAsync(Actor actor)
        {
            var statement = @"update actors set firstName=@par1, lastName=@par2, Gender=@par3, Biography=@par4, ImgUrl=@par5 where Id=@par6";

            var paramtersDefinition = @"@par1 nvarchar(20),@par2 nvarchar(20),@par3 nvarchar(20),@par4 nvarchar(max),@par5 nvarchar(450),@par6 int";

            var paramtersValues = @$"@par1='{actor.FirstName}',@par2='{actor.LastName}',@par3='{actor.Gender}',@par4='{actor.Biography}',@par5='{actor.ImgUrl}',@par6={actor.Id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }
    }
}
