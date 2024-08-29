using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maui.RevenueCat.InAppBilling.Services;
using Tsundoku.Repository;
using Tsundoku.Utility;
using Tsundoku.Views;

namespace Tsundoku.ViewModels;

public partial class RegisterPageViewModel : BaseViewModel
{
    private IBookInfoRepository _bookInfoRepository;
    private IRevenueCatBilling _revenueCatBilling;
    private SettingsPreferences _settingsPreferences;
    public RegisterPageViewModel(IBookInfoRepository bookInfoRepository, IRevenueCatBilling revenueCatBilling, SettingsPreferences settingsPreferences)
    {
        _bookInfoRepository = bookInfoRepository;
        _revenueCatBilling = revenueCatBilling;
        _settingsPreferences = settingsPreferences;
    }

    [ObservableProperty]
    string _isbn = string.Empty;

    [RelayCommand]
    async Task SearchAsync()
    {
        if (IsBusy || string.IsNullOrEmpty(Isbn)) return;
        try
        {
            IsBusy = true;
            Isbn = Isbn.Replace("-", "");
            if (!IsbnUtility.IsIsbnCode(Isbn)) return;
            string isbnCode = Isbn;
            if (Isbn.Length == 13 )
            {
                isbnCode = IsbnUtility.GetIsbn10(Isbn);
            }

            var bookCount = await _bookInfoRepository.GetAllBooksCountAsync();
            var vm = new ConfirmBookViewModel(isbnCode, _bookInfoRepository, _revenueCatBilling, _settingsPreferences);
            await Shell.Current.CurrentPage.ShowPopupAsync(new ConfirmBookView(vm));

            var afterBookCount = await _bookInfoRepository.GetAllBooksCountAsync();
            if (bookCount != afterBookCount)
            {
                Isbn = string.Empty;
            }
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