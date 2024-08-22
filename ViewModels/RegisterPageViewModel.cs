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

    [ObservableProperty]
    string _imageUrl = string.Empty;

    [RelayCommand]
    async Task SearchAsync()
    {
        try
        {
            if (Isbn.Length != 10 && Isbn.Length != 13) return;
            var isbnUtility = new ISBN();
            string isbnCode = Isbn;
            if (Isbn.Length == 13)
            {
                isbnCode = isbnUtility.ConvertISBN(Isbn).Replace("-", "");
            }

            ImageUrl = $"https://images.amazon.com/images/P/{isbnCode}.jpg";
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex);
        }
    }

    [RelayCommand]
    async Task NavigateToCameraPageAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            await Shell.Current.GoToAsync($"{nameof(CameraPageView)}", true);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        finally
        {
            IsBusy = false;
        }
    }
}
