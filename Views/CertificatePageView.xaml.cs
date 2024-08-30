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

    protected override void OnSizeAllocated(double width, double height)
    {
        base.OnSizeAllocated(width, height);
        _vm.GridItemSpan = Math.Max((int)width / 400, 2);
    }
}