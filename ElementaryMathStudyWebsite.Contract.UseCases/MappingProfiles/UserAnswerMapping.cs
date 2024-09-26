using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.UserAnswerDtos;
using ElementaryMathStudyWebsite.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles
{
    public class UserAnswerMapping : Profile
    {
        public UserAnswerMapping()
        {
            // Mapping between UserAnswer and UserAnswerDTO
            CreateMap<UserAnswer, UserAnswerDTO>();

            // Mapping between UserAnswer and UserAnswerWithDetailsDTO
            CreateMap<UserAnswer, UserAnswerWithDetailsDTO>()
                .ForMember(dest => dest.QuestionContent, opt => opt.MapFrom(src => src.Question != null ? src.Question.QuestionContext : "Unknown Question"))
                .ForMember(dest => dest.UserFullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "Unknown User"))
                .ForMember(dest => dest.OptionAnswer, opt => opt.MapFrom(src => src.Option != null ? src.Option.Answer : "Unknown Answer"));

            CreateMap<UserAnswerCreateDTO, UserAnswerWithDetailsDTO>()
                .ForMember(dest => dest.QuestionContent, opt => opt.Ignore())  
                .ForMember(dest => dest.UserFullName, opt => opt.Ignore())     
                .ForMember(dest => dest.OptionAnswer, opt => opt.Ignore());
        }
    }
}
