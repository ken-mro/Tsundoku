using Tsundoku.ViewModels;

namespace Tsundoku.Views;

public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage(MainPageViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }

    private void OnCounterClicked(object sender, EventArgs e)
    {
        count++;
    }
}
