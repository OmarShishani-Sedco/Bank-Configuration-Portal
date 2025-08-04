using AutoMapper;
using Bank_Configuration_Portal.Mappings;

namespace Bank_Configuration_Portal.App_Start
{
    public static class AutoMapperConfig
    {
        public static MapperConfiguration GetMapperConfiguration()
        {
            return new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperProfile>();
            });
        }
    }
}
