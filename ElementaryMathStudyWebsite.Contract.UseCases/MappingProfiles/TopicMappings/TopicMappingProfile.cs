using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Entity;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles.TopicMappings
{
    public class TopicMappingProfile : Profile
    {
        public TopicMappingProfile()
        {
            // Define the mapping configuration

            // Mapping for Topic to TopicViewDto
            CreateMap<Topic, TopicViewDto>()
                .ForMember(dest => dest.QuizName, opt => opt.MapFrom(src => src.Quiz != null ? src.Quiz.QuizName : string.Empty))
                .ForMember(dest => dest.ChapterName, opt => opt.MapFrom(src => src.Chapter != null ? src.Chapter.ChapterName : string.Empty));

            // Mapping for Topic to TopicAdminViewDto
            CreateMap<Topic, TopicAdminViewDto>()
                .ForMember(dest => dest.QuizName, opt => opt.MapFrom(src => src.Quiz != null ? src.Quiz.QuizName : string.Empty))
                .ForMember(dest => dest.ChapterName, opt => opt.MapFrom(src => src.Chapter != null ? src.Chapter.ChapterName : string.Empty))
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
                    : null));


            CreateMap<Topic, TopicDeleteDto>()
                .ForMember(dest => dest.QuizName, opt => opt.MapFrom(src => src.Quiz != null ? src.Quiz.QuizName : string.Empty))
                .ForMember(dest => dest.ChapterName, opt => opt.MapFrom(src => src.Chapter != null ? src.Chapter.ChapterName : string.Empty))
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
                .ForMember(dest => dest.DeleteName, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.DeletedBy != null && context.Items["DeleteUser"] is User deleteUser
                    ? deleteUser.FullName
                    : null))
                .ForMember(dest => dest.DeletePhone, opt => opt.MapFrom((src, dest, destMember, context) =>
                    src.DeletedBy != null && context.Items["DeleteUser"] is User deleteUser
                    ? deleteUser.PhoneNumber
                    : null));
        }
    }
}
