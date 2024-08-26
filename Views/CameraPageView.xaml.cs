using CommunityToolkit.Maui.Views;
using ISBNUtility;
using Tsundoku.Repository;
using Tsundoku.ViewModels;
using ZXing.Net.Maui;

namespace Tsundoku.Views;

public partial class CameraPageView : ContentPage
{
    CameraPageViewModel _vm;
    public CameraPageView(IBookInfoRepository bookInfoRepository)
    {
        InitializeComponent();
        BindingContext = _vm = new CameraPageViewModel(bookInfoRepository);
        cameraBarcodeReaderView.Options = new BarcodeReaderOptions
        {
            Formats = BarcodeFormats.OneDimensional,
            AutoRotate = true,
        };
        cameraBarcodeReaderView.CameraLocation = CameraLocation.Rear;
    }

    private void CameraBarcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        _vm.ShowConfirmationPopup(sender, e);
        //var code = e.Results?.FirstOrDefault()?.Value;
        //if (code is null) return;
        //if (code.Length != 10 && code.Length != 13) return;
        //if (code.Length == 13)
        //{
        //    var isbn = new ISBN();
        //    code = isbn.ConvertISBN(code).Replace("-", "");
        //}
        //var vm = new ConfirmBookViewModel(code);
        //Dispatcher.DispatchAsync(async () => await Shell.Current.CurrentPage.ShowPopupAsync(new ConfirmBookView(vm)));
    }
}