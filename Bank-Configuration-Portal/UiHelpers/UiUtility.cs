using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bank_Configuration_Portal.UiHelpers
{
    public static class UiUtility
    {
        public static bool AreObjectsEqual<T>(T obj1, T obj2, params string[] excludeProperties)
        {
            if (obj1 == null || obj2 == null) return false;

            var type = typeof(T);
            var props = type.GetProperties()
                            .Where(p => !excludeProperties.Contains(p.Name));

            foreach (var prop in props)
            {
                var val1 = prop.GetValue(obj1);
                var val2 = prop.GetValue(obj2);
                if (!object.Equals(val1, val2))
                    return false;
            }
            return true;
        }

    }
}