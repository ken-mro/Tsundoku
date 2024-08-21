using CommunityToolkit.Mvvm.ComponentModel;

namespace Tsundoku.ViewModels;

public partial class BaseViewModel : ObservableObject
{
    [ObservableProperty]
    string _title = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotBusy))]
    bool _isBusy = false;

    public bool IsNotBusy => !IsBusy;
}