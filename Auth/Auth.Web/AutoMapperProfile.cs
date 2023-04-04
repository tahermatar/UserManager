using Auth.Web.Models;
using Auth.Web.ViewModel;
using AutoMapper;

namespace Auth.Web
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //CreateMap<User, UserViewModel>().ForMember(x => x.UserEmail, x => x.MapFrom(x => x.Email));
            //CreateMap<User, UserViewModel>().ForMember(x => x.UserName, x => x.Ignore());
            CreateMap<User, UserViewModel>();
            CreateMap<CreateUserViewModel, User>();
        }
    }
}
