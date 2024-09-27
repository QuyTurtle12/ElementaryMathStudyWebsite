using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Core.Entity;
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
            CreateMap<Chapter, ChapterAdminViewDto>()
                .ForMember(dest => dest.CreatorName, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.CreatedBy != null && context.Items["CreatedUser"] is User createdUser
                    ? createdUser.FullName
                    : null))
                .ForMember(dest => dest.CreatorPhone, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.CreatedBy != null && context.Items["CreatedUser"] is User createdUser
                    ? createdUser.PhoneNumber
                    : null))
                .ForMember(dest => dest.LastUpdatedPersonName, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.LastUpdatedBy != null && context.Items["UpdatedUser"] is User updatedUser
                    ? updatedUser.FullName
                    : null))
                .ForMember(dest => dest.LastUpdatedPersonPhone, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.LastUpdatedBy != null && context.Items["UpdatedUser"] is User updatedUser
                    ? updatedUser.PhoneNumber
                    : null))
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.Subject != null && context.Items["Subject"] is Subject subject
                    ? subject.SubjectName
                    : null))
                .ForMember(dest => dest.QuizName, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.QuizId != null && context.Items["Quiz"] is Quiz quiz
                    ? quiz.QuizName
                    : null));
        }
    }
}
