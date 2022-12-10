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
    public class MoviesGeneresRepository:QueryManager,IMoviesGeneresRepository
    {
        private readonly string connectionString;
        public MoviesGeneresRepository(string connectionString)
        {
            this.connectionString = connectionString;  
        }

        public async Task<bool> AddMovieGenereAsync(MovieGenere movieGenere)
        {
            var sql = $@"insert into moviesGeneres(movieId,genereId)values({movieGenere.MovieId},{movieGenere.GenereId})";

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<bool> AddMoviesGeneresAsync(IEnumerable<MovieGenere> moviesGeneres)
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append("insert into MoviesGeneres(MovieId, GenereId) values");
            var cnt = 1;
            foreach(var movieGenere in moviesGeneres)
            {
                stringBuilder.Append($"({movieGenere.MovieId},{movieGenere.GenereId})");
                if ( cnt++ != moviesGeneres.Count())
                {
                    stringBuilder.Append(",");
                }
            }

            var rowsAffected = await ExecuteQueryAsync(stringBuilder.ToString(), connectionString);

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateMovieGenereAsync(MovieGenere movieGenere)
        {
            var sql = $@"update moviesGeneres set genereId = {movieGenere.GenereId} where movieId ={movieGenere.MovieId}";

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<bool> RemoveMovieGenereAsync(MovieGenere movieGenere)
        {
            var sql = $@"delete from moviesGeneres where genereId = {movieGenere.GenereId} and movieId ={movieGenere.MovieId}";

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<MovieGenere> GettMovieGenereAsync(int movieId, int genereId)
        {
            var sql = $"select movieId,genereId from moviesGeneres where movieId ={movieId} and genereId = {genereId}";

            MovieGenere movieGenere = null;
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
                            movieGenere = new MovieGenere
                            {
                                MovieId = Convert.ToInt32(reader[0]),
                                GenereId = Convert.ToInt32(reader[1])
                            };
                        }
                    }
                }
            }
            catch
            {
                //...
            }

            return movieGenere;

        }
    }
}
