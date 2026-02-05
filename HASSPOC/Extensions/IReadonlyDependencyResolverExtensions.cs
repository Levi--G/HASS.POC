using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HASSPOC.Extensions
{
    internal static class IReadonlyDependencyResolverExtensions
    {
        public static T GetRequiredService<T>(this IReadonlyDependencyResolver readonlyDependencyResolver)
        {
            return AppLocator.Current.GetService<T>() ?? throw new ApplicationException($"Required service {typeof(T).FullName} not found");
        }
    }
}
