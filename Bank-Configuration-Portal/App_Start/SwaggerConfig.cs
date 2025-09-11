using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using Swashbuckle.Application;
using Newtonsoft.Json.Converters;
using System.Net.Http;
using Bank_Configuration_Portal.Filters.Swagger;

namespace Bank_Configuration_Portal
{
    public class SwaggerConfig
    {
        public static void Register()
        {
            var config = GlobalConfiguration.Configuration;

            var json = config.Formatters.JsonFormatter.SerializerSettings;
            if (!json.Converters.OfType<StringEnumConverter>().Any())
                json.Converters.Add(new StringEnumConverter());

            config.EnableSwagger(c =>
            {
                c.SingleApiVersion("v1", "Bank Web APIs");
                c.PrettyPrint();
                c.ApiKey("Bearer").Name("Authorization").In("header")
                 .Description("Format: Bearer {token}");
                c.OperationFilter<RequireAuthOnSecuredOps>();
                c.Schemes(new[] { "https", "http" });

                var bin = HttpContext.Current.Server.MapPath("~/bin");
                foreach (var xml in Directory.EnumerateFiles(bin, "*.xml"))
                {
                    c.IncludeXmlComments(xml);
                }

                c.DescribeAllEnumsAsStrings();
            })
                 .EnableSwaggerUi(u =>
                 {
                     var asm = typeof(SwaggerConfig).Assembly;
                     u.CustomAsset("index", asm, "Bank_Configuration_Portal.SwaggerUi.index.html");

                     u.DocumentTitle("Bank Web APIs");
                     u.DisableValidator();
                 }); ;
           
        }
    }
}
