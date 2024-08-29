using CommunityToolkit.Maui.Views;
using Tsundoku.ViewModels;

namespace Tsundoku.Views;

public partial class PayWallView : Popup
{
	public PayWallView(PayWallViewModel vm)
	{
		InitializeComponent();
        vm.Popup = this;
        BindingContext = vm;
    }
}