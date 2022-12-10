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
    public class MoviesDirectorsRepository:QueryManager,IMoviesDirectorsRepository
    {
        private readonly string connectionString;
        public MoviesDirectorsRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<bool> AddMovieDirectorAsync(MovieDirector movieDirector)
        {
            var sql = $@"insert into moviesDirectors(movieId,directorId)values({movieDirector.MovieId},{movieDirector.DirectorId})";

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<MovieDirector> GetMovieDirectorAsync(int movieId, int directorId)
        {
            var sql = $"select movieId,directorId from moviesDirectors where movieId ={movieId} and directorId = {directorId}";

            MovieDirector movieDirector = null;
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
                            movieDirector = new MovieDirector
                            {
                                MovieId = Convert.ToInt32(reader[0]),
                                DirectorId = Convert.ToInt32(reader[1])
                            };
                        }
                    }
                }
            }
            catch
            {
                //...
            }

            return movieDirector;
        }

        public async Task<bool> RemoveMovieDirectorAsync(MovieDirector movieDirector)
        {
            var sql = $@"delete from moviesDirectors where directorId = {movieDirector.DirectorId} and movieId ={movieDirector.MovieId}";

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateMovieDirectorAsync(MovieDirector movieDirector)
        {
            var sql = $@"update moviesDirectors set directorId = {movieDirector.DirectorId} where movieId ={movieDirector.MovieId}";

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }
    }
}
