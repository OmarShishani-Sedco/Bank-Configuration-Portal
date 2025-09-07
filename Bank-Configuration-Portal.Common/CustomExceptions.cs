using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.Common
{
    public class CustomConcurrencyModifiedException : SystemException
    {
        public CustomConcurrencyModifiedException(string message) : base(message) { }
    }

    public class CustomConcurrencyDeletedException : SystemException
    {
        public CustomConcurrencyDeletedException(string message) : base(message) { }
    }

    public sealed class DatabaseTimeoutException : Exception
    {
        public DatabaseTimeoutException(string message, Exception inner) : base(message, inner) { }
    }

}
