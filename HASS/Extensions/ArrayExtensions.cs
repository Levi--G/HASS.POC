using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HASS.Extensions
{
    public static class ArrayExtensions
    {
        public static bool Contains(this string[] strings, string item, StringComparison comparison)
        {
            return strings.Contains(item, CaseInsensitiveEqualityComparer.Instance);
        }

        public class CaseInsensitiveEqualityComparer : IEqualityComparer<string>
        {
            public static CaseInsensitiveEqualityComparer Instance = new CaseInsensitiveEqualityComparer();

            public bool Equals(string? x, string? y)
            {
                if (x == null && y == null || ReferenceEquals(x, y))
                {
                    return true;
                }
                if (x == null || y == null)
                {
                    return false;
                }
                return x.Equals(y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode([DisallowNull] string obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
