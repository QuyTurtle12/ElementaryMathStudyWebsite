using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles.ProgressMappings
{
    public class QuizMappingProfile : Profile
    {
        public QuizMappingProfile()
        {
            CreateMap<Quiz, QuizMainViewDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuizName, opt => opt.MapFrom(src => src.QuizName))
                .ForMember(dest => dest.ChapterName, opt => opt.MapFrom(src => src.Chapter != null ? src.Chapter.ChapterName : string.Empty))
                .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic != null ? src.Topic.TopicName : string.Empty))

                .ForMember(dest => dest.CreatorName, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items["CreatedUser"] is List<User> users
                        ? users.FirstOrDefault(u => u.Id == src.CreatedBy)?.FullName ?? string.Empty
                        : string.Empty))

                .ForMember(dest => dest.CreatorPhone, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items["CreatedUser"] is List<User> users
                        ? users.FirstOrDefault(u => u.Id == src.CreatedBy)?.PhoneNumber ?? string.Empty
                        : string.Empty))

                .ForMember(dest => dest.LastUpdatedPersonName, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items["UpdatedUser"] is List<User> users
                        ? users.FirstOrDefault(u => u.Id == src.LastUpdatedBy)?.FullName ?? string.Empty
                        : string.Empty))

                .ForMember(dest => dest.LastUpdatedPersonPhone, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items["UpdatedUser"] is List<User> users
                        ? users.FirstOrDefault(u => u.Id == src.LastUpdatedBy)?.PhoneNumber ?? string.Empty
                        : string.Empty));

            CreateMap<QuizMainViewDto, Quiz>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuizName, opt => opt.MapFrom(src => src.QuizName))
                .ForMember(dest => dest.Criteria, opt => opt.MapFrom(src => src.Criteria))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items["CreatorUser"] is List<User> users
                        ? users.FirstOrDefault(u => u.Id == src.CreatedBy)?.FullName ?? string.Empty
                        : string.Empty))
                .ForMember(dest => dest.LastUpdatedBy, opt => opt.MapFrom((src, dest, destMember, context) =>
                    context.Items["UpdatedUser"] is List<User> users
                        ? users.FirstOrDefault(u => u.Id == src.LastUpdatedBy)?.FullName ?? string.Empty
                        : string.Empty))
                .ForMember(dest => dest.CreatedTime, opt => opt.MapFrom(src => src.CreatedTime))
                .ForMember(dest => dest.LastUpdatedTime, opt => opt.MapFrom(src => src.LastUpdatedTime));




            CreateMap<Quiz, QuizViewDto>()
                .ForMember(dest => dest.ChapterName, opt => opt.MapFrom(src => src.Chapter != null ? src.Chapter.ChapterName : string.Empty))
                .ForMember(dest => dest.TopicName, opt => opt.MapFrom(src => src.Topic != null ? src.Topic.TopicName : string.Empty));
        }
    }
}