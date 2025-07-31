using System;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bank_Configuration_Portal.App_Start
{
    public static class DIConfig
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void RegisterServices()
        {
            var services = new ServiceCollection();

            // TODO: Register services here
            // services.AddSingleton<ILoginService, LoginService>();
            // services.AddScoped<IBankService, BankService>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public static T GetService<T>() => ServiceProvider.GetService<T>();
    }
}