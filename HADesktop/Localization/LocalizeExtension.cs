using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Text;

namespace HADesktop.Localization
{
    public class LocExtension(string key) : MarkupExtension, IObservable<string>, IDisposable
    {
        readonly LocalizationService service = AppLocator.Current.GetRequiredService<LocalizationService>();

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this.ToBinding();
        }

        private IObserver<string>? _observer;

        public IDisposable Subscribe(IObserver<string> observer)
        {
            _observer = observer;
            _observer.OnNext(service.GetLocalizedByKey(key) ?? key);
            service.LanguageChanged += OnLanguageChanged;

            return this;
        }

        public void Dispose()
        {
            service.LanguageChanged -= OnLanguageChanged;
            _observer = null;
        }

        private void OnLanguageChanged()
        {
            _observer?.OnNext(service.GetLocalizedByKey(key) ?? key);
        }
    }
}
