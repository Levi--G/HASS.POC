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

    SourceList<IIntegration> integrations = new();

    [Reactive]
    ReadOnlyObservableCollection<IIntegration> filteredIntegrations;

    [Reactive]
    string? searchText;

    [Reactive]
    IIntegration? selectedItem;

    public ModuleDialogViewModel()
    {
        integrations.AddRange(AppLocator.Current.GetServices<IIntegration>().Where(i => !i.Enabled));
        var filter = this.WhenAnyValue(v => v.SearchText).Throttle(TimeSpan.FromMilliseconds(300)).Select<string?, Func<IIntegration, bool>>(s => i => s == null || i.Name.Contains(s, StringComparison.OrdinalIgnoreCase) || i.Description.Contains(s, StringComparison.OrdinalIgnoreCase));
        integrations.Connect().Filter(filter).Bind(out filteredIntegrations).Subscribe();
        this.WhenAnyValue(v=>v.SelectedItem).Where(i => i != null).Subscribe(Selected);
    }

    public void Close()
    {
        RequestClose?.Invoke(this, null);
    }

    private void Selected(IIntegration? selection)
    {
        RequestClose?.Invoke(this, selection);
    }
}