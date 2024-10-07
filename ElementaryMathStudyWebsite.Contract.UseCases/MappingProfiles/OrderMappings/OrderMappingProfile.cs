using AutoMapper;
using ElementaryMathStudyWebsite.Contract.UseCases.DTOs;
using ElementaryMathStudyWebsite.Core.Repositories.Entity;

namespace ElementaryMathStudyWebsite.Contract.UseCases.MappingProfiles.OrderMappings
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile() 
        {
            // Define the mapping configuration

            // Mapping for OrderDetail to OrderDetailViewDto
            CreateMap<OrderDetail, OrderDetailViewDto>()
                .ForMember(dest => dest.SubjectName, opt => opt.MapFrom(src => src.Subject != null ? src.Subject.SubjectName : string.Empty))
                .ForMember(dest => dest.StudentName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty));

            // Mapping for Order to OrderViewDto
            CreateMap<Order, OrderViewDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.CreatedTime))
                .ForPath(dest => dest.Details, opt => opt.MapFrom(src => src.OrderDetails));

            // Mapping for Order to OrderAdminViewDto
            CreateMap<Order, OrderAdminViewDto>()
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.CreatedTime))
                .ForPath(dest => dest.Details, opt => opt.MapFrom(src => src.OrderDetails))
                .ForMember(dest => dest.CreatorName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.CreatorPhone, opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : string.Empty))
                .ForMember(dest => dest.LastUpdatedPersonName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.LastUpdatedPersonPhone, opt => opt.MapFrom(src => src.User != null ? src.User.PhoneNumber : string.Empty));


        }
    }
}
