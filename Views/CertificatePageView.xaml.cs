using Tsundoku.ViewModels;

namespace Tsundoku.Views;

public partial class CertificatePageView : ContentPage
{
	public CertificatePageView(CertificatePageViewModel vm)
	{
		InitializeComponent();
        BindingContext = vm;
    }
}