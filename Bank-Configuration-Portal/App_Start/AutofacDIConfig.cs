using Autofac;
using Autofac.Integration.Mvc; // This is the key namespace from the NuGet package
using AutoMapper;
using Bank_Configuration_Portal.BLL;
using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.DAL.DAL;
using Bank_Configuration_Portal.DAL.Interfaces;
using System.Web.Mvc;

namespace Bank_Configuration_Portal.App_Start
{
    public static class AutofacConfig
    {
        public static IContainer Container { get; private set; }

        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(MvcApplication).Assembly);
            builder.RegisterType<BankDAL>().As<IBankDAL>().InstancePerRequest();
            builder.RegisterType<BankManager>().As<IBankManager>().InstancePerRequest();
            builder.RegisterType<BranchDAL>().As<IBranchDAL>().InstancePerRequest();
            builder.RegisterType<BranchManager>().As<IBranchManager>().InstancePerRequest();
            builder.RegisterType<ServiceDAL>().As<IServiceDAL>().InstancePerRequest();
            builder.RegisterType<ServiceManager>().As<IServiceManager>().InstancePerRequest();
            builder.RegisterType<CounterDAL>().As<ICounterDAL>().InstancePerRequest();
            builder.RegisterType<CounterManager>().As<ICounterManager>().InstancePerRequest();
            builder.RegisterType<UserDAL>().As<IUserDAL>().InstancePerRequest();
            builder.RegisterType<UserManager>().As<IUserManager>().InstancePerRequest();

            builder.Register(context =>
            {
                var config = AutoMapperConfig.GetMapperConfiguration();
                return config;
            }).AsSelf().SingleInstance();

            builder.Register(c =>
            {
                var context = c.Resolve<MapperConfiguration>();
                return context.CreateMapper();
            }).As<IMapper>().SingleInstance();


            Container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(Container));
        }
    }
}