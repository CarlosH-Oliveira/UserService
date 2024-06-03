using AutoMapper;
using UserService.Models.User;
using UserService.Models.User.DTOS;

namespace UserService.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDTO, User>();

            CreateMap<User, ReadUserDTO>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(user => user.Id))
                .ForMember(dto => dto.Name, opt => opt.MapFrom(user => user.Name))
                .ForMember(dto => dto.Email, opt => opt.MapFrom(user => user.Email))
                .ForMember(dto => dto.CreatedAt, opt => opt.MapFrom(user => user.CreatedAt))
                .ForMember(dto => dto.LastLogin, opt => opt.MapFrom(user => user.LastLogin))
                .ForMember(dto => dto.Status, opt => opt.MapFrom(user => user.Status));
        }
    }
}
