using AutoMapper;
using MoviesWebApplication.DAL.Data;
using MoviesWebApplication.Web.Areas.Admin.Models.UsersController;
using MoviesWebApplication.Web.Areas.Identity.Models;
using MoviesWebApplication.Web.WebOptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace MoviesWebApplication.Web.AutoMapperProfiles
{
    internal class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<User, RegisterViewModel>();

            CreateMap<RegisterViewModel, User>().ForMember(user => user.UserName, options => options.MapFrom(model => model.Email));

            CreateMap<User,UsersDetailsViewModel>().ForMember(model=>model.Name,options=>options.MapFrom(user=>$"{user.FirstName} {user.LastName}"))
                                            .ForMember(model => model.Role, options => options.MapFrom(model => model.Role.Name))
                                            .ForMember(model=>model.Email,options=>options.MapFrom(user=>user.UserName));

            CreateMap<AddANewUserViewModel, User>().ForMember(user => user.UserName, options => options.MapFrom(model => model.Email))
                                                    .ForMember(user => user.EmailConfirmed, options => options.MapFrom(model => 1));

            CreateMap<AccountOption, User>().ForMember(user=>user.IsBlocked,options=>options.MapFrom(account=>false))
                                            .ForMember(user => user.EmailConfirmed, options => options.MapFrom(account => true));
        }
    }
}
