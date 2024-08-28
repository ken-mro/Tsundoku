using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tsundoku.Repository;
using Tsundoku.Utility;
using Tsundoku.Views;

namespace Tsundoku.ViewModels;

public partial class RegisterPageViewModel : BaseViewModel
{
    private IBookInfoRepository _bookInfoRepository;
    public RegisterPageViewModel(IBookInfoRepository bookInfoRepository)
    {
        _bookInfoRepository = bookInfoRepository;
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
            if (IsbnUtility.IsIsbnCode(Isbn)) return;
            string isbnCode = Isbn;
            if (Isbn.Length == 13 )
            {
                isbnCode = IsbnUtility.GetIsbn10(Isbn);
            }

            await Shell.Current.CurrentPage.ShowPopupAsync(new ConfirmBookView(new ConfirmBookViewModel(isbnCode, _bookInfoRepository)));
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