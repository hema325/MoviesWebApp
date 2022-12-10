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
    public class MoviesRepository:QueryManager,IMoviesRepository
    {
        private readonly string connectionString;
        public MoviesRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<bool> AddMovieAsync(Movie movie)
        {
            var statement = @"insert into movies(title,releaseYear,duration,language,country,MovieUrl,moviePosterUrl)
                              values(@par1,@par2,@par3,@par4,@par5,@par6,@par7)";

            var paramtersDefinition = @"@par1 nvarchar(100),@par2 datetime2,@par3 int,@par4 nvarchar(20),@par5 nvarchar(20),@par6 varchar(450),@par7 varchar(450)";

            var paramtersValues = @$"@par1='{movie.Title}',@par2='{movie.ReleaseYear}',@par3={movie.Duration},@par4='{movie.Language}',@par5='{movie.Country}',@par6='{movie.MovieUrl}',@par7='{movie.MoviePosterUrl}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<int> CountMoviesAsync()
        {
            var sql = @"select count(*) from movies";

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

        public async Task<int> CountMoviesByTitleAsync(string name)
        {
            var statement = @"select count(*) from movies where title like @par1";

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
                //...
            }

            return -1;
        }

        public async Task<IEnumerable<Movie>> GetAllMoviesAsync(int skip, int take)
        {
            var sql = @$"select id,title,releaseYear,duration,language,country,MovieUrl,moviePosterUrl,
                               (select cast(sum(stars) as decimal(5,2))/(count(*)*5)*100 from moviesReviewers where MovieId=Id) as rate from movies
                               order by id 
                               offset {skip} rows
                               fetch next {take} rows only";

            var movies = new List<Movie>();
            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    var cmd = new SqlCommand(sql, cn);
                    await cn.OpenAsync();
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while(await reader.ReadAsync())
                        {
                            movies.Add(new Movie {
                                Id = Convert.ToInt32(reader[0]),
                                Title = reader[1].ToString(),
                                ReleaseYear= Convert.ToDateTime(reader[2]),
                                Duration= Convert.ToInt32(reader[3]),
                                Language = reader[4].ToString(),
                                Country = reader[5].ToString(),
                                MovieUrl = reader[6].ToString(),
                                MoviePosterUrl = reader[7].ToString(),
                                Rate = Convert.ToDouble(reader[8] == DBNull.Value ? null: reader[8])
                            });
                        }
                    }
                }
            }
            catch
            {
                //......
            }

            return movies;

        }

        public async Task<IEnumerable<Movie>> GetAllMoviesHeighstRatedAsync(int skip, int take)
        {
            var sql = @$"select id,title,releaseYear,duration,language,country,MovieUrl,moviePosterUrl,
                               (select cast(sum(stars) as decimal(5,2))/(count(*)*5)*100 from moviesReviewers where MovieId=Id) as rate from movies
                               order by rate desc 
                               offset {skip} rows
                               fetch next {take} rows only";

            var movies = new List<Movie>();
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
                            movies.Add(new Movie
                            {
                                Id = Convert.ToInt32(reader[0]),
                                Title = reader[1].ToString(),
                                ReleaseYear = Convert.ToDateTime(reader[2]),
                                Duration = Convert.ToInt32(reader[3]),
                                Language = reader[4].ToString(),
                                Country = reader[5].ToString(),
                                MovieUrl = reader[6].ToString(),
                                MoviePosterUrl = reader[7].ToString(),
                                Rate = Convert.ToDouble(reader[8] == DBNull.Value ? null : reader[8])
                            });
                        }
                    }
                }
            }
            catch
            {
                //......
            }

            return movies;

        }

        public async Task<IEnumerable<Movie>> GetAllMoviesByTitleAsync(int skip, int take, string title)
        {
            var statement = @$"select id,title,releaseYear,duration,language,country,MovieUrl,moviePosterUrl,
                               (select cast(sum(stars) as decimal(5,2))/(count(*)*5)*100 from moviesReviewers where MovieId=Id) as rate from movies 
                               where title like @par1
                               order by id 
                               offset {skip} rows
                               fetch next {take} rows only";

            var paramtersDefinition = @"@par1 nvarchar(100)";

            var paramtersValues = @$"@par1='%{title}%'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);



            var movies = new List<Movie>();
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
                            movies.Add(new Movie
                            {
                                Id = Convert.ToInt32(reader[0]),
                                Title = reader[1].ToString(),
                                ReleaseYear = Convert.ToDateTime(reader[2]),
                                Duration = Convert.ToInt32(reader[3]),
                                Language = reader[4].ToString(),
                                Country = reader[5].ToString(),
                                MovieUrl = reader[6].ToString(),
                                MoviePosterUrl = reader[7].ToString(),
                                Rate = Convert.ToDouble(reader[8] == DBNull.Value ? null : reader[8])
                            });
                        }
                    }
                }
            }
            catch
            {
                //......
            }

            return movies;

        }

        public async Task<Movie> GetMovieByTitleAsync(string title)
        {
            var statement = @$"select id,title,releaseYear,duration,language,country,MovieUrl,moviePosterUrl from movies where title = @par1";

            var paramtersDefinition = @"@par1 nvarchar(100)";

            var paramtersValues = @$"@par1='{title}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);



            Movie movie = null;
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
                            movie = new Movie
                            {
                                Id = Convert.ToInt32(reader[0]),
                                Title = reader[1].ToString(),
                                ReleaseYear = Convert.ToDateTime(reader[2]),
                                Duration = Convert.ToInt32(reader[3]),
                                Language = reader[4].ToString(),
                                Country = reader[5].ToString(),
                                MovieUrl = reader[6].ToString(),
                                MoviePosterUrl = reader[7].ToString()
                            };
                        }
                    }
                }
            }
            catch
            {
                //......
            }

            return movie;

        }

        public async Task<Movie> GetMovieByIdAsync(int id)
        {
            var sql = @$"select id,title,releaseYear,duration,language,country,MovieUrl,moviePosterUrl,
                        (select cast(sum(stars) as decimal(5,2))/(count(*)*5)*100 from moviesReviewers where MovieId=Id) as rate from movies
                        where id = {id} ";


            Movie movie = null;
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
                            movie=new Movie
                            {
                                Id = Convert.ToInt32(reader[0]),
                                Title = reader[1].ToString(),
                                ReleaseYear = Convert.ToDateTime(reader[2]),
                                Duration = Convert.ToInt32(reader[3]),
                                Language = reader[4].ToString(),
                                Country = reader[5].ToString(),
                                MovieUrl = reader[6].ToString(),
                                MoviePosterUrl = reader[7].ToString(),
                                Rate = Convert.ToDouble(reader[8] == DBNull.Value ? null : reader[8])
                            };
                        }
                    }
                }
            }
            catch
            {
                //......
            }

            return movie;

        }

        public async Task<bool> RemoveMovieById(int id)
        {
            var sql = @$"delete from movies where id = {id}";

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;

        }

        public async Task<bool> UpdateMovieAsync(Movie movie)
        {
            var statement = @$"update movies set title=@par1,releaseYear=@par2,duration=@par3,language=@par4,country=@par5,
                               moviePosterUrl=@par7 where id = @par8";

            var paramtersDefinition = @"@par1 nvarchar(100),@par2 datetime2,@par3 int,@par4 nvarchar(20),@par5 nvarchar(20),@par7 varchar(450),@par8 int";

            var paramtersValues = $@"@par1='{movie.Title}',@par2='{movie.ReleaseYear}',@par3={movie.Duration},@par4='{movie.Language}',
                                     @par5='{movie.Country}',@par7='{movie.MoviePosterUrl}',@par8={movie.Id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;

        }

    }
}
