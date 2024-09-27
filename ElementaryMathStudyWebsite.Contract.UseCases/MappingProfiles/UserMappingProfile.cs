using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ResponseDto;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;


namespace ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            // Define the mapping configuration
            CreateMap<User, UserResponseDto>();
            CreateMap<Role, RoleDto>();
            CreateMap<RequestRole, Role>();
            CreateMap<CreateUserDto, User>();
            CreateMap<User, UserProfile>();
            CreateMap<User, UpdateProfileDto>();
            CreateMap<User, UserResponseDto>()
            .ForPath(dest => dest.Role.RoleId, opt => opt.MapFrom(src => src.Role.Id))
            .ForPath(dest => dest.Role.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));

            CreateMap<Role, RoleDto>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.RoleName));

            CreateMap<UpdateUserDto, User>()
            .ForMember(dest => dest.Password, opt => opt.Ignore()) // Ignore password if it's not being updated
            .ForMember(dest => dest.Id, opt => opt.Ignore()); // Ignore ID if it's not being updated
        }

    }
}
