using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;


namespace ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles.QuestionMappings
{
    public class QuestionMappingProfile : Profile
    {
        public QuestionMappingProfile()
        {
            CreateMap<Question, QuestionMainViewDto>()

                .ForMember(dest => dest.QuestionContext, opt => opt.MapFrom(src => src.QuestionContext))
                .ForMember(dest => dest.QuizName, opt => opt.MapFrom(src => src.Quiz != null ? src.Quiz.QuizName : string.Empty))
                .ForMember(dest => dest.QuizId, opt => opt.MapFrom(src => src.Quiz != null ? src.Quiz.Id : string.Empty))

                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.Id : string.Empty))
                .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.FullName: string.Empty))
                .ForMember(dest => dest.CreatorPhone, opt => opt.MapFrom(src => src.CreatedByUser != null ? src.CreatedByUser.PhoneNumber : string.Empty))

                .ForMember(dest => dest.LastUpdatedBy, opt => opt.MapFrom(src => src.LastUpdatedByUser != null ? src.LastUpdatedByUser.Id : string.Empty))
                .ForMember(dest => dest.LastUpdatedPersonName, opt => opt.MapFrom(src => src.LastUpdatedByUser != null ? src.LastUpdatedByUser.FullName : string.Empty))
                .ForMember(dest => dest.LastUpdatedPersonPhone, opt => opt.MapFrom(src => src.LastUpdatedByUser != null ? src.LastUpdatedByUser.PhoneNumber : string.Empty))
                
                .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.MapFrom(src => src.LastUpdatedTime));

            CreateMap<Question, QuestionViewDto>()
               .ForMember(dest => dest.QuizName, opt => opt.MapFrom(src => src.Quiz != null ? src.Quiz.QuizName : string.Empty))
               .ForMember(dest => dest.QuizId, opt => opt.MapFrom(src => src.Quiz != null ? src.Quiz.Id : string.Empty));
        }
    }
}
