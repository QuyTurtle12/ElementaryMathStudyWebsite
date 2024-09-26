using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.RequestDto;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserDto.ResponseDto;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;


namespace ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles
{
    public class SubjectMapping : Profile
    {
        public SubjectMapping()
        {
            // Define the mapping configuration
            CreateMap<Subject, SubjectDTO>();
            CreateMap<Subject, SubjectAdminViewDTO>()
            .ForMember(dest => dest.CreaterName, opt => opt.MapFrom((src, dest, destMember, context) =>
                src.CreatedBy != null ? context.Items["CreatedUser"] is User createdUser ? createdUser.FullName : null : null))
            .ForMember(dest => dest.CreaterPhone, opt => opt.MapFrom((src, dest, destMember, context) =>
                src.CreatedBy != null ? context.Items["CreatedUser"] is User createdUser ? createdUser.PhoneNumber : null : null))
            .ForMember(dest => dest.UpdaterName, opt => opt.MapFrom((src, dest, destMember, context) =>
                src.LastUpdatedBy != null ? context.Items["UpdatedUser"] is User updatedUser ? updatedUser.FullName : null : null))
            .ForMember(dest => dest.UpdaterPhone, opt => opt.MapFrom((src, dest, destMember, context) =>
                src.LastUpdatedBy != null ? context.Items["UpdatedUser"] is User updatedUser ? updatedUser.PhoneNumber : null : null));
            CreateMap<SubjectCreateDTO, SubjectDTO>();
        }

    }
}
