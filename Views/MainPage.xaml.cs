using Tsundoku.ViewModels;

namespace Tsundoku.Views;

public partial class MainPage : ContentPage
{
    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
