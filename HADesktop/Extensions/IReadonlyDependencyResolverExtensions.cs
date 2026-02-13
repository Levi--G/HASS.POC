using Avalonia.Controls;

namespace HADesktop.Extensions
{
    internal static class IReadonlyDependencyResolverExtensions
    {
        public static T GetRequiredService<T>(this IReadonlyDependencyResolver readonlyDependencyResolver)
        {
            if (Design.IsDesignMode && readonlyDependencyResolver.GetService<HaDesktopApp>() == null)
            {
                new HaDesktopApp().StartupInDesignMode().Wait();
            }
            return AppLocator.Current.GetService<T>() ?? throw new ApplicationException($"Required service {typeof(T).FullName} not found");
        }
    }
}
