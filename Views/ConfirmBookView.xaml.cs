using CommunityToolkit.Maui.Views;
using Tsundoku.ViewModels;

namespace Tsundoku.Views;

public partial class ConfirmBookView : Popup
{
	public ConfirmBookView(ConfirmBookViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
		vm.Popup = this;
	}
}