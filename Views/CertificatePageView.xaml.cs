using Tsundoku.ViewModels;

namespace Tsundoku.Views;

public partial class CertificatePageView : ContentPage
{
    private readonly CertificatePageViewModel _vm;
	public CertificatePageView(CertificatePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = _vm = vm;
    }

    // Books are marked as read from other tabs and popups, so reload on every visit.
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _vm.Init();
    }

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        var numMoreThan1 = Math.Max((int)width / 300, 1);
        _vm.GridItemSpan = Math.Min(numMoreThan1, 2);
    }
}