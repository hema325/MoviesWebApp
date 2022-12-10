using Microsoft.Extensions.Options;
using MoviesWebApplication.Web.DALOptions;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesWebApplication.DAL.DataBaseManagement.DataBaseBuilder
{
    public class DataBaseBuilder : QueryManager, IDataBaseBuilder
    {
        private readonly ConnectionStringsOption connectionStringsOption;
        public DataBaseBuilder(IOptions<ConnectionStringsOption> options)
        {
            connectionStringsOption = options.Value;
        }

        public async Task<DataBaseBuilder> Build()
        {
            await AddMoviesDataBase();
            await AddUsersTableAsync();
            await AddRolesTableAsync();
            await AddUserRolesTableAsync();
            await AddUserTokensTableAsync();
            await AddActorsTableAsync();
            await AddMoviesTableAsync();
            await AddMoviesActorsTableAsync();
            await AddGeneresTableAsync();
            await AddMoviesGeneresTableAsync();
            await AddDirectorsTableAsync();
            await AddMoviesDirectorsTableAsync();
            await AddReviewersTableAsync();
            await AddMoviesReviewersTableAsync();
            await RemoveUnValidTokens();
            return this;
        }

        private async Task<DataBaseBuilder> AddMoviesDataBase()
        {
            var sql = @"if db_Id('Movies') is null
                        create database Movies";

            await ExecuteQueryAsync(sql, connectionStringsOption.ConnectionWithoutDataBase);

            return this;
        }

        private async Task<DataBaseBuilder> AddUsersTableAsync()
        {
            var sql = @"if object_id('Users') is null
                           create table [Users](
                           Id int primary key identity,
                           FirstName nvarchar(20),
                           LastName nvarchar(20),
                           UserName nvarchar(256) unique,
                           Email nvarchar(256) unique,
                           EmailConfirmed bit default 0,
                           PasswordHash nvarchar(max),
                           IsBlocked bit default 0,
                           ImgUrl varchar(450)
                           )";

            await ExecuteQueryAsync(sql, connectionStringsOption.DefaultConnection);

            return this;
        }

        private async Task<DataBaseBuilder> AddRolesTableAsync()
        {
            var sql = @"if object_id('Roles') is null
                           create table [Roles] (
                           Id int primary key identity,
                           Name nvarchar(256) unique
                           )";

            await ExecuteQueryAsync(sql, connectionStringsOption.DefaultConnection);

            return this;

        }

        private async Task<DataBaseBuilder> AddUserRolesTableAsync()
        {
            var sql = @"if object_id('UserRoles') is null
                           create table [UserRoles] (
                           UserId int references Users(Id) on update cascade on delete cascade ,
                           RoleId int references Roles(Id) on update cascade on delete cascade,
                           primary key(UserId,RoleId)
                           )";

            await ExecuteQueryAsync(sql, connectionStringsOption.DefaultConnection);

            return this;
        }

        private async Task<DataBaseBuilder> AddUserTokensTableAsync()
        {
            var sql = @"if object_id('UserTokens') is null
                        create table [UserTokens] (
                        UserId int references Users(Id) on update cascade on delete cascade  index idx_UserTokensUserId clustered(UserId),
                        Token nvarchar(max),
                        ValidTill dateTime2(2)
                        )";

            await ExecuteQueryAsync(sql, connectionStringsOption.DefaultConnection);

            return this;
        }

        private async Task<DataBaseBuilder> AddActorsTableAsync()
        {
            var sql = @"if object_id('Actors') is null
                          create Table [Actors] (
                          Id int primary key identity,
                          FirstName nvarchar(20),
                          LastName nvarchar(20),
                          Gender nvarchar(20) ,
                          Biography nvarchar(max),
                          ImgUrl varchar(450)
                          )";

            await ExecuteQueryAsync(sql, connectionStringsOption.DefaultConnection);

            return this;

        }

        private async Task<DataBaseBuilder> AddMoviesTableAsync()
        {
            var sql = @"if object_id('Movies') is null
                           create table [Movies] (
                           Id int primary key identity,
                           Title nvarchar(100) unique,
                           ReleaseYear DateTime2(2) default getDate(),
                           Duration int,
                           [Language] nvarchar(20),
                           Country nvarchar(20),
                           MovieUrl varchar(450),
                           MoviePosterUrl varchar(450),
                           index idx_Movies nonClustered(Title)
                           )";

            await ExecuteQueryAsync(sql, connectionStringsOption.DefaultConnection);

            return this;

        }

        private async Task<DataBaseBuilder> AddMoviesActorsTableAsync()
        {
            var sql = @"if object_id('MoviesActors') is null
                        create table [MoviesActors] (
                        ActorId int references Actors(Id) on update cascade on delete cascade,
                        MovieId int references Movies(Id) on update cascade on delete cascade,
                        [Role] nvarchar(100),
                        primary key(ActorId,MovieId)
                        )";

            await ExecuteQueryAsync(sql, connectionStringsOption.DefaultConnection);

            return this;

        }

        private async Task<DataBaseBuilder> AddGeneresTableAsync()
        {
            var sql = @"if object_id('Generes') is null
                        create table [Generes](
                        id int primary key identity,
                        [Name] nvarchar(100) unique
                        )";

            await ExecuteQueryAsync(sql, connectionStringsOption.DefaultConnection);

            return this;

        }

        private async Task<DataBaseBuilder> AddMoviesGeneresTableAsync()
        {
            var sql = @"if object_id('MoviesGeneres') is null
                           create table [MoviesGeneres](
                           MovieId int references Movies(Id) on update cascade on delete cascade,
                           GenereId int references Generes(Id) on update cascade on delete cascade,
                           primary key(MovieId,GenereId)
                           )";

            await ExecuteQueryAsync(sql, connectionStringsOption.DefaultConnection);

            return this;
        }

        private async Task<DataBaseBuilder> AddDirectorsTableAsync()
        {
            var sql = @"if object_id('Directors') is null
                        create table [Directors] (
                        Id int primary key identity,
                        FirstName nvarchar(20),
                        LastName nvarchar(20),
                        Gender nvarchar(20),
                        Biography nvarchar(max),
                        ImgUrl varchar(450)
                        )";

            await ExecuteQueryAsync(sql, connectionStringsOption.DefaultConnection);

            return this;
        }

        private async Task<DataBaseBuilder> AddMoviesDirectorsTableAsync()
        {
            var sql = @"if object_id('MoviesDirectors') is null
                           create table [MoviesDirectors] (
                           MovieId int references Movies(Id) on update cascade on delete cascade,
                           DirectorId int references Directors(Id) on update cascade on delete cascade,
                           primary key(MovieId,DirectorsId)
                           )";

            await ExecuteQueryAsync(sql, connectionStringsOption.DefaultConnection);

            return this;

        }

        private async Task<DataBaseBuilder> AddReviewersTableAsync()
        {
            var sql = @"if object_id('Reviewers') is null
                        create table [Reviewers] (
                        Id int primary key identity,
                        FirstName nvarchar(20),
                        LastName nvarchar(20),
                        Gender nvarchar(20),
                        Biography nvarchar(max),
                        ImgUrl varchar(450)
                        )";

            await ExecuteQueryAsync(sql, connectionStringsOption.DefaultConnection);

            return this;

        }

        private async Task<DataBaseBuilder> AddMoviesReviewersTableAsync()
        {
            var sql = @"if object_id('MoviesReviewers') is null
                        create table [MoviesReviewers] (
                        MovieId int references Movies(Id) on update cascade on delete cascade,
                        ReviewerId int references Reviewers(Id) on update cascade on delete cascade,
                        Stars int,
                        primary Key(MovieId,ReviewerId)
                        )";

            await ExecuteQueryAsync(sql, connectionStringsOption.DefaultConnection);

            return this;

        }


        private async Task<DataBaseBuilder> RemoveUnValidTokens()
        {
            var sql = @" delete from userTokens where getDate()>=validTill";

            await ExecuteQueryAsync(sql, connectionStringsOption.DefaultConnection);

            return this;

        }

    }
}
