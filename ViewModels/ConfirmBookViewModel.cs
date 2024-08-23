using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISBNUtility;

namespace Tsundoku.ViewModels;

public partial class ConfirmBookViewModel : BaseViewModel
{
	public ConfirmBookViewModel(string isbn)
	{
        _isbn10 = isbn;
        ImageUrl = $"https://images.amazon.com/images/P/{_isbn10}.jpg";
    }

    [ObservableProperty]
    string _imageUrl = string.Empty;

	string _isbn10;

    public Popup Popup;

    [RelayCommand]
    async Task StackBookAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            await Task.Delay(2000);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            IsBusy = false;
            await Popup.CloseAsync();
        }
    }
}