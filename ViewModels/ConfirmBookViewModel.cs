using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maui.RevenueCat.InAppBilling.Services;
using Tsundoku.Repository;
using Tsundoku.Views;

namespace Tsundoku.ViewModels;

public partial class ConfirmBookViewModel : BaseViewModel
{
    private IBookInfoRepository _bookInfoRepository;
    private IRevenueCatBilling _revenueCat;
    private SettingsPreferences _settingsPreferences;

    public ConfirmBookViewModel(string isbn10, IBookInfoRepository bookInfoRepository, IRevenueCatBilling revenueCatBilling, SettingsPreferences settingsPreferences)
    {
        _revenueCat = revenueCatBilling;
        _settingsPreferences = settingsPreferences;

        _bookInfoRepository = bookInfoRepository;
        _isbn10 = isbn10;
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

            var allBooks = await _bookInfoRepository.GetAllBooksAsync();
            var StackedBookCount = allBooks.Where(b => !b.Read).Count();

            if (StackedBookCount >= 10)
            {
                await TryToOfferSubscription();
                if (!_settingsPreferences.GetIsSubscribed()) return;
            }

            var result = await _bookInfoRepository.AddBookInfoAsync(_isbn10);
            if (result > 0)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Completed", "The book stacked successfully", "OK");
            }
            else
            {
                throw new Exception("Failed to stack the book");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.CurrentPage.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            await Popup.CloseAsync();
        }
    }

    private async Task TryToOfferSubscription()
    {
        var customerInfo = await _revenueCat.GetCustomerInfo();
        var isSubscribed = customerInfo?.ActiveSubscriptions?.Count > 0;
        _settingsPreferences.SetIsSubscribed(isSubscribed);
        if (!_settingsPreferences.GetIsSubscribed())
        {
            await Shell.Current.CurrentPage.ShowPopupAsync(new PayWallView(new PayWallViewModel(_revenueCat, _settingsPreferences)));
        }
    }
}