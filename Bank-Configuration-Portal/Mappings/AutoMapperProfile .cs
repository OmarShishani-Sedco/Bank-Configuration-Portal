using AutoMapper;
using Bank_Configuration_Portal.Models;
using Bank_Configuration_Portal.Models.Models;

namespace Bank_Configuration_Portal.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<BranchModel, BranchViewModel>().ReverseMap();
            CreateMap<ServiceModel, ServiceViewModel>().ReverseMap();
            CreateMap<CounterModel, CounterViewModel>()
                       .ForMember(dest => dest.SelectedServiceIds, opt => opt.MapFrom(src => src.AllocatedServiceIds))
                       .ForMember(dest => dest.AllActiveServices, opt => opt.Ignore()) // The controller populates this list
                       .ReverseMap()
                       .ForMember(dest => dest.AllocatedServiceIds, opt => opt.MapFrom(src => src.SelectedServiceIds));
        }
    }
}
