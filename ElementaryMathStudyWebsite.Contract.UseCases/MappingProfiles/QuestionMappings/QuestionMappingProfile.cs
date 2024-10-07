using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
using ElementaryMathStudyWebsite.Core.Utils;

namespace ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles.QuestionMappings
{
    public class QuestionMappingProfile : Profile
    {
        public QuestionMappingProfile()
        {

            CreateMap<Question, QuestionMainViewDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuestionContext, opt => opt.MapFrom(src => src.QuestionContext))
                .ForMember(dest => dest.QuizId, opt => opt.MapFrom(src => src.Quiz != null ? src.Quiz.Id : string.Empty))
                .ForMember(dest => dest.QuizName, opt => opt.MapFrom(src => src.Quiz != null ? src.Quiz.QuizName : string.Empty))

                .ForMember(dest => dest.CreatorName, opt => opt.MapFrom((src, dest, destMember, context) =>
                    GetUserFullName(src.CreatedBy, context.Items["CreatedUsers"] as List<User>)))
                .ForMember(dest => dest.CreatorPhone, opt => opt.MapFrom((src, dest, destMember, context) =>
                    GetUserPhoneNumber(src.CreatedBy, context.Items["CreatedUsers"] as List<User>)))
                .ForMember(dest => dest.LastUpdatedPersonName, opt => opt.MapFrom((src, dest, destMember, context) =>
                    GetUserFullName(src.LastUpdatedBy, context.Items["UpdatedUsers"] as List<User>)))
                .ForMember(dest => dest.LastUpdatedPersonPhone, opt => opt.MapFrom((src, dest, destMember, context) =>
                    GetUserPhoneNumber(src.LastUpdatedBy, context.Items["UpdatedUsers"] as List<User>)));



            CreateMap<Question, QuestionViewDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.QuestionContext, opt => opt.MapFrom(src => src.QuestionContext))
                .ForMember(dest => dest.QuizId, opt => opt.MapFrom(src => src.Quiz != null ? src.Quiz.Id : string.Empty))
                .ForMember(dest => dest.QuizName, opt => opt.MapFrom(src => src.Quiz != null ? src.Quiz.QuizName : string.Empty));
        }

        private string GetUserFullName(string? userId, List<User>? users)
        {
            return users?.FirstOrDefault(u => u.Id == userId)?.FullName ?? string.Empty;
        }

        private string GetUserPhoneNumber(string? userId, List<User>? users)
        {
            return users?.FirstOrDefault(u => u.Id == userId)?.PhoneNumber ?? string.Empty;
        }
    }
}
