using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles.ChapterMappings
{
    public class ChapterMappingProfile : Profile
    {
        public ChapterMappingProfile() 
        {
            // Mapping for Chapter to ChapterViewDto
            CreateMap<Chapter, ChapterViewDto>()
                    .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectName : string.Empty))
                    .ForMember(dest => dest.QuizName, opt => opt.MapFrom(src => src.Quiz != null ? src.Quiz.QuizName : string.Empty));

            // Mapping for Chapter to ChapterAdminViewDto
            //CreateMap<Chapter, ChapterAdminViewDto>()
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            //    .ForMember(dest => dest.Number, opt => opt.MapFrom(src => src.Number))
            //    .ForMember(dest => dest.ChapterName, opt => opt.MapFrom(src => src.ChapterName))
            //    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            //    .ForMember(dest => dest.SubjectId, opt => opt.MapFrom(src => src.SubjectId))
            //    .ForMember(dest => dest.QuizId, opt => opt.MapFrom(src => src.QuizId))
            //    .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
            //    .ForMember(dest => dest.CreatorPhone, opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : string.Empty))
            //    .ForMember(dest => dest.LastUpdatedPersonName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
            //    .ForMember(dest => dest.LastUpdatedPersonPhone, opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : string.Empty));
        }
    }
}
