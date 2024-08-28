using CommunityToolkit.Maui.Views;
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
            Multiple = true,
        };
        cameraBarcodeReaderView.CameraLocation = CameraLocation.Rear;
    }

    private void CameraBarcodeReaderView_BarcodesDetected(object sender, ZXing.Net.Maui.BarcodeDetectionEventArgs e)
    {
        _vm.ShowConfirmationPopup(sender, e);
    }
}