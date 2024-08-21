using Tsundoku.ViewModels;

namespace Tsundoku.Views;

public partial class RegisterPageView : ContentPage
{
	public RegisterPageView()
	{
		InitializeComponent();
		BindingContext = new RegisterPageViewModel();
	}
}