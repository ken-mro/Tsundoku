using CommunityToolkit.Maui.Views;
using ISBNUtility;
using Tsundoku.Views;
using ZXing.Net.Maui;

namespace Tsundoku.ViewModels;

public partial class CameraPageViewModel : BaseViewModel
{
    public CameraPageViewModel()
    {
    }

    public async Task ShowConfirmationPopup(object sender, BarcodeDetectionEventArgs e)
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var code = e.Results?.FirstOrDefault()?.Value;
            if (string.IsNullOrEmpty(code)) return;
            if (code.Length != 10 && code.Length != 13) return;
            string isbnCode = code;
            if (code.Length == 13)
            {
                var isbnUtility = new ISBN();
                isbnCode = isbnUtility.ConvertISBN(code).Replace("-", "");
            }

            await Device.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.CurrentPage.ShowPopupAsync(new ConfirmBookView(new ConfirmBookViewModel(isbnCode)));
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