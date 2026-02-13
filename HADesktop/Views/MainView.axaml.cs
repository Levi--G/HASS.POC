using Avalonia;
using Avalonia.Controls;
using Ursa.Controls;

namespace HADesktop.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        AppLocator.CurrentMutable.RegisterConstant(new WindowNotificationManager(TopLevel.GetTopLevel(this)) { MaxItems = 3 });
    }
}
