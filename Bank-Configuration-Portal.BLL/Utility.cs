using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank_Configuration_Portal.BLL
{
    public static class Utility
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

                if (val1 is System.Collections.IEnumerable enumerable1 &&
                    val2 is System.Collections.IEnumerable enumerable2)
                {
                    var list1 = enumerable1.Cast<object>().ToList();
                    var list2 = enumerable2.Cast<object>().ToList();

                    if (!list1.SequenceEqual(list2))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!object.Equals(val1, val2))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
