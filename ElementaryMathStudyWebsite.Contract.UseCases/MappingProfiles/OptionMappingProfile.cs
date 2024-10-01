using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;
namespace ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles
{
    public class OptionMappingProfile : Profile
    {
        public OptionMappingProfile()
        {
            // Define mapping configuration

            //Map Option to OptionViewDto
            CreateMap<Option,OptionViewDto>();
        }
    }
}
