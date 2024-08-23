using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ISBNUtility;
using Tsundoku.Views;

namespace Tsundoku.ViewModels;

public partial class RegisterPageViewModel : BaseViewModel
{
    public RegisterPageViewModel()
    {

    }

    [ObservableProperty]
    string _isbn = string.Empty;

    [RelayCommand]
    async Task SearchAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            Isbn = Isbn.Replace("-", "");
            if (Isbn.Length != 10 && Isbn.Length != 13) return;
            string isbnCode = Isbn;
            if (Isbn.Length == 13)
            {
                var isbnUtility = new ISBN();
                isbnCode = isbnUtility.ConvertISBN(Isbn).Replace("-", "");
            }

            await Shell.Current.CurrentPage.ShowPopupAsync(new ConfirmBookView(new ConfirmBookViewModel(isbnCode)));
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task NavigateToCameraPageAsync()
    {
        await Shell.Current.GoToAsync($"{nameof(CameraPageView)}", true);
    }
}