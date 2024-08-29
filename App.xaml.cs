using Maui.RevenueCat.InAppBilling.Services;
using Tsundoku.Repository;

namespace Tsundoku;

public partial class App : Application
{
    private readonly IRevenueCatBilling _revenueCat;
    public App(IRevenueCatBilling revenueCatBilling)
    {
        InitializeComponent();
        _revenueCat = revenueCatBilling;

        MainPage = new AppShell();
    }

    protected override void OnStart()
    {
        var revenueCatApiKey = string.Empty;

#if __ANDROID__
    revenueCatApiKey = Constants.RevenueCatApiKeyAndroid;
#elif __IOS__
    revenueCatApiKey = Constants.RevenueCatApiKeyIos;
#endif

        _revenueCat.Initialize(revenueCatApiKey);

        base.OnStart();
    }
}
