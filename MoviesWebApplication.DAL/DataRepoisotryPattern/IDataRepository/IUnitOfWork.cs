
using MoviesWebApplication.DAL.DataRepoisotryPattern.IDataRepository;
using MoviesWebApplication.DLL.IDataRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.IDataRepository
{
    public interface IUnitOfWork
    {
        IUserRepository Users { get; }
        IUserTokenRepository UserTokens { get; }
        IRoleRepository Roles { get; }
        IActorRepository Actors { get; }
        IDirectorRepository Directors { get; }
        IGenereRepository Generes { get; }
        IMoviesRepository Movies { get; }
        IReviewerRepository Reviewers { get; }
        IMoviesActorsRepository MoviesActors { get; }
        IMoviesReviewersRepository MoviesReviewers { get; }
        IMoviesGeneresRepository MoviesGeneres { get; }
        IMoviesDirectorsRepository MoviesDirectors { get; }
        void ChangeConnectionString(string connectionString);
    }
}
