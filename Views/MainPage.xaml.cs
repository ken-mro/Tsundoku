using Tsundoku.ViewModels;

namespace Tsundoku.Views;

public partial class MainPage : ContentPage
{
    private readonly MainPageViewModel _vm;
    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        _vm.GridItemSpan = Math.Max((int)width / 400, 2);
    }
}
