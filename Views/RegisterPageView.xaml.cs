using System.Runtime.ExceptionServices;
using Tsundoku.ViewModels;
using ZXing.Net.Maui;

namespace Tsundoku.Views;

public partial class RegisterPageView : ContentPage
{
	public RegisterPageView(RegisterPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}