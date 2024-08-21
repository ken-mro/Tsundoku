using CommunityToolkit.Mvvm.ComponentModel;

namespace Tsundoku.ViewModels;

public partial class RegisterPageViewModel : BaseViewModel
{
    public RegisterPageViewModel()
    {

    }

    [ObservableProperty]
    string _isbn = string.Empty;
}
