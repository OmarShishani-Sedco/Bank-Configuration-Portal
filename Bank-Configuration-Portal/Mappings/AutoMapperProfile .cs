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
            CreateMap<CounterModel, CounterViewModel>().ReverseMap();
        }
    }
}
