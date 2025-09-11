using Swashbuckle.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace Bank_Configuration_Portal.Filters.Swagger
{
    /// <summary>
    /// Adds a Bearer security requirement to every operation that is not [AllowAnonymous].
    /// </summary>
    public class RequireAuthOnSecuredOps : IOperationFilter
    {
        public void Apply(Operation op, SchemaRegistry _, ApiDescription desc)
        {
            var isAnon =
                desc.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any() ||
                desc.ActionDescriptor.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any();

            if (isAnon) return;

            if (op.security == null)
                op.security = new List<IDictionary<string, IEnumerable<string>>>();

            op.security.Add(new Dictionary<string, IEnumerable<string>>
            {
                { "Bearer", Enumerable.Empty<string>() }
            });
        }
    }
}