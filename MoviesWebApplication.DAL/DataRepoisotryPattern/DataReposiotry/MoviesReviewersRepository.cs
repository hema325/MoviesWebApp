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
    public class MoviesReviewersRepository:QueryManager,IMoviesReviewersRepository
    {
        private readonly string connectionString;
        public MoviesReviewersRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<bool> AddMovieReviewerAsync(MovieReviewer movieReviewer)
        {
            var statement = @$"insert into moviesReviewers(ReviewerId,MovieId,Stars) values(@par1,@par2,@par3)";

            var paramtersDefinition = @"@par1 int,@par2 int,@par3 int";

            var paramtersValues = $@"@par1={movieReviewer.ReviewerId},@par2={movieReviewer.MovieId},@par3={movieReviewer.Stars}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<MovieReviewer> GetMovieReviewerAsync(int movieId,int reviewerId)
        {
            var sql = $@"select ReviewerId,MovieId,Stars from moviesReviewers where movieId = {movieId} and reviewerId = {reviewerId}";

            MovieReviewer movieReviewer = null;
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
                            movieReviewer = new MovieReviewer
                            {
                                ReviewerId = Convert.ToInt32(reader[0]),
                                MovieId = Convert.ToInt32(reader[1]),
                                Stars = Convert.ToInt32(reader[2])
                            };
                        }
                    }
                }
            }
            catch
            {
                //...
            }

            return movieReviewer;

        }

        public async Task<bool> RemoveMovieReviewerAsync(MovieReviewer movieReviewer)
        {
            var sql = @$"delete from moviesReviewers where reviewerId = {movieReviewer.ReviewerId} and movieId={movieReviewer.MovieId}";

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;

        }

        public async Task<bool> UpdateMovieReviewerAsync(MovieReviewer movieReviewer)
        {
            var statement = @$"update moviesReviewers set stars = @par3 where ReviewerId=@par1 and MovieId = @par2";

            var paramtersDefinition = @"@par1 int,@par2 int,@par3 int";

            var paramtersValues = $@"@par1={movieReviewer.ReviewerId},@par2={movieReviewer.MovieId},@par3={movieReviewer.Stars}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }
    }
}
