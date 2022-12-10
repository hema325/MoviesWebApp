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
    public class ReviewerRepository:QueryManager,IReviewerRepository
    {
        private readonly string connectionString;
        public ReviewerRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<bool> AddReviewerAsync(Reviewer reviewer)
        {
            var statement = @"insert into reviewers(firstName,lastName,Gender,Biography,ImgUrl) values(@par1,@par2,@par3,@par4,@par5)";

            var paramtersDefinition = @"@par1 nvarchar(20),@par2 nvarchar(20),@par3 nvarchar(20),@par4 nvarchar(max),@par5 nvarchar(450)";

            var paramtersValues = @$"@par1='{reviewer.FirstName}',@par2='{reviewer.LastName}',@par3='{reviewer.Gender}',@par4='{reviewer.Biography}',@par5='{reviewer.ImgUrl}'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<int> CountReviewerAsync()
        {
            var sql = @"select count(*) from reviewers";

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

        public async Task<int> CountReviewerByNameAsync(string name)
        {
            var statement = @"select count(*) from reviewers where firstName like @par1 or lastName like @par1";

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

        public async Task<IEnumerable<Reviewer>> GetAllReviewersAsync(int skip, int take)
        {
            var statement = @"select id, firstName, lastName, Gender, Biography, ImgUrl from reviewers
                              order by Id
                              offset @par1 rows
                              fetch next @par2 rows only";

            var paramtersDefinition = @"@par1 int,@par2 int";

            var paramtersValues = @$"@par1={skip},@par2= {take}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var reviewers = new List<Reviewer>();

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
                            reviewers.Add(new Reviewer
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
            return reviewers;
        }

        public async Task<IEnumerable<Reviewer>> GetAllReviewersAsync()
        {
            var sql = $@"select id, firstName, lastName, Gender, Biography, ImgUrl from reviewers";

            var reviewers = new List<Reviewer>();

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
                            reviewers.Add(new Reviewer
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
            return reviewers;
        }

        public async Task<IEnumerable<Reviewer>> GetAllReviewersByNameAsync(int skip, int take, string name)
        {
            var statement = @"select id, firstName, lastName, Gender, Biography, ImgUrl from reviewers where firstName like @par3 or lastName like @par3
                              order by Id
                              offset @par1 rows
                              fetch next @par2 rows only";

            var paramtersDefinition = @"@par1 int,@par2 int,@par3 nvarchar(40)";

            var paramtersValues = @$"@par1={skip},@par2= {take},@par3='%{name}%'";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var reviewers = new List<Reviewer>();

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
                            reviewers.Add(new Reviewer
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
            return reviewers;
        }

        public async Task<Reviewer> GetReviewerByIdAsync(int id)
        {
            var statement = @"select id, firstName, lastName, Gender, Biography, ImgUrl from reviewers where id = @par1";

            var paramtersDefinition = @"@par1 int";

            var paramtersValues = @$"@par1={id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            Reviewer reviewer = null;
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
                            reviewer = new Reviewer
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
            return reviewer;
        }

        public async Task<bool> RemoveReviewerById(int id)
        {
            var statement = @"delete from reviewers where id = @par1";

            var paramtersDefinition = @"@par1 int";

            var paramtersValues = @$"@par1={id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<bool> UpdateReviewerAsync(Reviewer reviewer)
        {
            var statement = @"update reviewers set firstName=@par1, lastName=@par2, Gender=@par3, Biography=@par4, ImgUrl=@par5 where Id=@par6";

            var paramtersDefinition = @"@par1 nvarchar(20),@par2 nvarchar(20),@par3 nvarchar(20),@par4 nvarchar(max),@par5 nvarchar(450),@par6 int";

            var paramtersValues = @$"@par1='{reviewer.FirstName}',@par2='{reviewer.LastName}',@par3='{reviewer.Gender}',@par4='{reviewer.Biography}',@par5='{reviewer.ImgUrl}',@par6={reviewer.Id}";

            var sql = GenerateSql(statement, paramtersDefinition, paramtersValues);

            var rowsAffected = await ExecuteQueryAsync(sql, connectionString);

            return rowsAffected > 0;
        }

        public async Task<IEnumerable<Reviewer>> GetAllReviewersForMovieAsync(int movieId)
        {
            var sql = @$"select id,firstName,lastName,Gender,Biography,ImgUrl,movieId,reviewerId,stars from reviewers as R 
                         join MoviesReviewers as MR on R.Id = MR.reviewerId where MR.MovieId={movieId}";

            var reviewers = new List<Reviewer>();

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
                            reviewers.Add(new Reviewer
                            {
                                Id = Convert.ToInt32(reader[0]),
                                FirstName = reader[1].ToString(),
                                LastName = reader[2].ToString(),
                                Gender = (Gender)Enum.Parse(typeof(Gender), reader[3].ToString()),
                                Biography = reader[4].ToString(),
                                ImgUrl = reader[5].ToString(),
                                MovieReviewer = new MovieReviewer
                                {
                                    MovieId = Convert.ToInt32(reader[6]),
                                    ReviewerId = Convert.ToInt32(reader[7]),
                                    Stars = Convert.ToInt32(reader[8])
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
            return reviewers;

        }
    }
}
