using CommunityToolkit.Maui.Views;
using Tsundoku.ViewModels;

namespace Tsundoku.Views;

public partial class PayWallView : Popup
{
	public PayWallView(PayWallViewModel vm)
	{
		InitializeComponent();
        // CommunityToolkit.Maui's popup treats Thickness.Zero as "unset" and falls back
        // to its default margin (30), so a 1-unit top margin is the closest we can get
        // to keeping the sheet flush with the screen edges.
        Margin = new Thickness(0, 1, 0, 0);
        vm.Popup = this;
        BindingContext = vm;
    }
}