using DynamicData;
using HADesktop.Integrations;
using Irihi.Avalonia.Shared.Contracts;
using ReactiveUI;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Ursa.Controls;

namespace HADesktop.ViewModels;

public partial class ModuleDialogViewModel : ViewModelBase, IDialogContext
{
    public event EventHandler<object?>? RequestClose;

    SourceList<IIntegrationRegistration> integrations = new();

    [Reactive]
    ReadOnlyObservableCollection<IIntegrationRegistration> filteredIntegrations;

    [Reactive]
    string? searchText;

    [Reactive]
    IIntegrationRegistration? selectedItem;
    AppSettingsService AppSettingsService = AppLocator.Current.GetRequiredService<AppSettingsService>();

    public ModuleDialogViewModel()
    {
        integrations.AddRange(AppSettingsService.GetPossibleIntegrationRegistrations());
        var filter = this.WhenAnyValue(v => v.SearchText).Throttle(TimeSpan.FromMilliseconds(300)).Select<string?, Func<IIntegrationRegistration, bool>>(s => i => s == null || i.Name.Contains(s, StringComparison.OrdinalIgnoreCase) || i.Description.Contains(s, StringComparison.OrdinalIgnoreCase));
        integrations.Connect().Filter(filter).Bind(out filteredIntegrations).Subscribe();
        this.WhenAnyValue(v => v.SelectedItem).Where(i => i != null).Subscribe(Selected);
    }

    public void Close()
    {
        RequestClose?.Invoke(this, null);
    }

    private void Selected(IIntegrationRegistration? selection)
    {
        RequestClose?.Invoke(this, selection);
    }
}