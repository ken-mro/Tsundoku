using ISBNUtility;
using Tsundoku.ViewModels;
using ZXing.Net.Maui;

namespace Tsundoku.Views;

public partial class CameraPageView : ContentPage
{
	public CameraPageView()
    {
        InitializeComponent();
        BindingContext = new CameraPageViewModel();
        cameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.OneDimensional,
            AutoRotate = true,
        };
        cameraBarcodeReaderView.CameraLocation = CameraLocation.Rear;
    }

    private void CameraBarcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        var code = e.Results?.FirstOrDefault()?.Value;
        if (code is null) return;
        if (code.Length != 10 && code.Length != 13) return;
        if (code.Length == 13)
        {
            var isbn = new ISBN();
            code = isbn.ConvertISBN(code).Replace("-", "");
        }
        Dispatcher.DispatchAsync(async () => await DisplayAlert("Barcode Detected", code, "OK"));
    }
}