using AutoMapper;
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
        }
    }
}
