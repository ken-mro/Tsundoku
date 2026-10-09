using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using Maui.RevenueCat.InAppBilling.Services;
using Tsundoku.Repository;
using Tsundoku.Resources;
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

    // The barcode usually stays in front of the camera after the popup closes, so remember
    // the last book shown to avoid reopening the popup for it over and over.
    private string _lastIsbn = string.Empty;

    // Must be called on the main thread.
    public async Task ShowConfirmationPopup(object sender, BarcodeDetectionEventArgs e)
    {
        if (IsBusy) return;
        var code = e.Results?.Where(c => IsbnUtility.IsIsbnCode(c.Value)).FirstOrDefault()?.Value;
        if (string.IsNullOrEmpty(code)) return;
        var isbnCode = IsbnUtility.GetIsbn10(code);
        if (isbnCode == _lastIsbn) return;
        try
        {
            IsBusy = true;
            _lastIsbn = isbnCode;
            var vm = new ConfirmBookViewModel(isbnCode, _bookInfoRepository, _revenueCatBilling, _settingsPreferences);
            await Shell.Current.CurrentPage.ShowPopupAsync(new ConfirmBookView(vm));
        }
        catch (Exception ex)
        {
            await Shell.Current.CurrentPage.DisplayAlertAsync(AppResources.Error, ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}