using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles.ProgressMappings
{
    public class QuizMappingProfile : Profile
    {
        public QuizMappingProfile()
        {
            // Mapping for Quiz to QuizMainViewDto
            CreateMap<Quiz, QuizMainViewDto>()
                .ForMember(dest => dest.ChapterName, opt => opt.MapFrom(src => src.Chapter != null ? src.Chapter.ChapterName : string.Empty))
                .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic != null ? src.Topic.TopicName : string.Empty))
                
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorName, opt => opt.Ignore())
                .ForMember(dest => dest.CreatorPhone, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedTime, opt => opt.Ignore())

                .ForMember(dest => dest.LastUpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedPersonName, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedPersonPhone, opt => opt.Ignore())
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.Ignore());

            CreateMap<Quiz, QuizViewDto>()
                .ForMember(dest => dest.ChapterName, opt => opt.MapFrom(src => src.Chapter != null ? src.Chapter.ChapterName : string.Empty))
                .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic != null ? src.Topic.TopicName : string.Empty));
        }


    }
}