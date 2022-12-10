using Microsoft.Extensions.Options;
using MoviesWebApplication.DAL.DataRepoisotryPattern.DataReposiotry;
using MoviesWebApplication.DAL.DataRepoisotryPattern.IDataRepository;
using MoviesWebApplication.DAL.IDataRepository;
using MoviesWebApplication.DLL.IDataRepository;
using MoviesWebApplication.Web.DALOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataReposiotry
{
    public class UnitOfWork:IUnitOfWork
    {
        private string ConnectionString;
        public IUserRepository Users { get; }
        public IUserTokenRepository UserTokens { get; }
        public IRoleRepository Roles { get; }
        public IActorRepository Actors { get; }
        public IDirectorRepository Directors { get; }
        public IGenereRepository Generes { get; }
        public IMoviesRepository Movies { get; }
        public IReviewerRepository Reviewers { get; }
        public IMoviesActorsRepository MoviesActors { get; }
        public IMoviesReviewersRepository MoviesReviewers { get; }
        public IMoviesGeneresRepository MoviesGeneres { get; }
        public IMoviesDirectorsRepository MoviesDirectors { get; }

        public UnitOfWork(IOptions<ConnectionStringsOption> options)
        {
            ConnectionString = options.Value.DefaultConnection; 
            Users = new UserRepository(ConnectionString);
            UserTokens = new UserTokenRepository(ConnectionString);
            Roles = new RoleRepository(ConnectionString);
            Actors = new ActorRepository(ConnectionString);
            Directors = new DirectorRepository(ConnectionString);
            Generes = new GenereRepository(ConnectionString);
            Movies = new MoviesRepository(ConnectionString);
            Reviewers = new ReviewerRepository(ConnectionString);
            MoviesActors = new MoviesActorsRepository(ConnectionString);
            MoviesReviewers = new MoviesReviewersRepository(ConnectionString);
            MoviesGeneres = new MoviesGeneresRepository(ConnectionString);
            MoviesDirectors = new MoviesDirectorsRepository(ConnectionString);
        }

        public void ChangeConnectionString(string connectionString)
        {
            ConnectionString = connectionString;
        }

    }
}
