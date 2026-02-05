using Avalonia.Controls;
using Avalonia.Controls.Templates;
using HASSPOC.Integrations;
using HASSPOC.ViewModels;
using HASSPOC.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HASSPOC.Services
{
    public class IntegrationViewLocator : IDataTemplate
    {
        Dictionary<Type, Func<Control>> views = new();

        public Control Build(object? data)
        {
            if (data != null && views.TryGetValue(data.GetType(), out var func))
            {
                return func();
            }
            return new TextBlock() { Text = $"Not Found: {data?.GetType().FullName}" };

        }

        public bool Match(object? data) => data is IIntegrationViewModel;

        public void Register(Type type, Func<Control> constructor)
        {
            views.Add(type, constructor);
        }
    }
}
