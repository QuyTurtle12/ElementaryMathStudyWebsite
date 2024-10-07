using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles.ResultMappings
{
    public class ResultMappingProfile : Profile
    {
        public ResultMappingProfile() 
        {
            // Define the mapping configuration

            // Mapping Result to ResultViewDto
            CreateMap<Result, ResultViewDto>()
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.Student!.FullName))
                .ForMember(dest => dest.QuizName, opt => opt.MapFrom(src => src.Quiz!.QuizName))
                .ForMember(dest => dest.Attempt, opt => opt.MapFrom(src => src.AttemptNumber));

            CreateMap<ResultCreateDto, Result>();
        }


    }
}
