using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles.ProgressMappings
{
    public class ProgressMappingProfile : Profile
    {
        public ProgressMappingProfile()
        {
            // Define the mapping configuration
            CreateMap<Progress, ProgressViewDto>()
           .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject.SubjectName)); // Adjust according to your Subject entity
        }
    }
}
