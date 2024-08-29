using CommunityToolkit.Maui.Views;
using Maui.RevenueCat.InAppBilling.Services;
using Tsundoku.Repository;
using Tsundoku.Utility;
using Tsundoku.Views;
using ZXing.Net.Maui;

namespace Tsundoku.ViewModels;

public partial class CameraPageViewModel : BaseViewModel
{
    private IBookInfoRepository _bookInfoRepository;
    private IRevenueCatBilling _revenueCatBilling;
    private SettingsPreferences _settingsPreferences;
    public CameraPageViewModel(IBookInfoRepository bookInfoRepository, IRevenueCatBilling revenueCatBilling, SettingsPreferences settingsPreferences)
    {
        _bookInfoRepository = bookInfoRepository;
        _revenueCatBilling = revenueCatBilling;
        _settingsPreferences = settingsPreferences;
    }

    public async Task ShowConfirmationPopup(object sender, BarcodeDetectionEventArgs e)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var code = e.Results?.Where(c => IsbnUtility.IsIsbnCode(c.Value)).FirstOrDefault()?.Value;
            if (string.IsNullOrEmpty(code)) return;
            if (code.Length != 10 && code.Length != 13) return;
            string isbnCode = code;
            if (code.Length == 13)
            {
                isbnCode = IsbnUtility.GetIsbn10(isbnCode);
            }

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var vm = new ConfirmBookViewModel(isbnCode, _bookInfoRepository, _revenueCatBilling, _settingsPreferences);
                await Shell.Current.CurrentPage.ShowPopupAsync(new ConfirmBookView(vm));
            });
        }
        catch (Exception ex)
        {

        }
        finally
        {
            IsBusy = false;
        }
    }
}