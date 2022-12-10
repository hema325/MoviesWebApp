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
    public class MoviesActorsRepository:QueryManager,IMoviesActorsRepository
    {
        private readonly string connectionString;
        public MoviesActorsRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }
        public async Task<bool> AddMovieActorAsync(MovieActor movieActor)
        {
            var statement = @$"insert into moviesActors(ActorId,MovieId,Role) values(@par1,@par2,@par3)";

            var paramtersDefinition = @"@par1 int,@par2 int,@par3 nvarchar(100)";

            var paramtersValues = $@"@par1={movieActor.ActorId},@par2={movieActor.MovieId},@par3='{movieActor.Role}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<MovieActor> GetMovieActorAsync(int movieId,int actorId)
        {
            var sql = @$"select ActorId,MovieId,Role from moviesActors where ActorId={actorId} and MovieId={movieId}";

            MovieActor movieActor = null;
            try
            {
                using(var cn = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand(sql, cn);
                    await cn.OpenAsync();
                    using(var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            movieActor = new MovieActor
                            {
                                ActorId = Convert.ToInt32(reader[0]),
                                MovieId = Convert.ToInt32(reader[1]),
                                Role = reader[2].ToString()
                            };
                        }
                    }
                }
            }
            catch
            {
                //...
            }

            return movieActor;

        }

        public async Task<bool> UpdateMovieActorAsync(MovieActor movieActor)
        {
            var statement = @$"update moviesActors set Role = @par3 where ActorId=@par1 and MovieId = @par2";

            var paramtersDefinition = @"@par1 int,@par2 int,@par3 nvarchar(100)";

            var paramtersValues = $@"@par1={movieActor.ActorId},@par2={movieActor.MovieId},@par3='{movieActor.Role}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<bool> RemoveMovieActorAsync(MovieActor movieActor)
        {
            var sql = @$"delete from moviesActors where movieId = {movieActor.MovieId} and actorId={movieActor.ActorId}";

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

    }
}
