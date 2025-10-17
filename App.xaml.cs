using CommunityToolkit.Maui.Views;
using Maui.RevenueCat.InAppBilling.Services;
using Tsundoku.Repository;
using Tsundoku.Resources;
using Tsundoku.Utility;
using Tsundoku.ViewModels;
using Tsundoku.Views;

namespace Tsundoku;

public partial class App : Application
{
    private readonly IRevenueCatBilling _revenueCat;
    private readonly IBookInfoRepository _bookInfoRepository;
    private readonly SettingsPreferences _settingsPreferences;

    public App(IRevenueCatBilling revenueCatBilling, IBookInfoRepository bookInfoRepository, SettingsPreferences settings)
    {
        InitializeComponent();
        _revenueCat = revenueCatBilling;
        _bookInfoRepository = bookInfoRepository;
        _settingsPreferences = settings;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }

    protected async override void OnStart()
    {
        var revenueCatApiKey = string.Empty;

#if __ANDROID__
    revenueCatApiKey = Constants.RevenueCatApiKeyAndroid;
#elif __IOS__
    revenueCatApiKey = Constants.RevenueCatApiKeyIos;
#endif

        _revenueCat.Initialize(revenueCatApiKey);

        base.OnStart();

        await TryToGetSharedContent();
    }

    protected async override void OnResume()
    {
        base.OnResume();

        await Task.Delay(1000);
        await TryToGetSharedContent();
    }

    private async Task TryToGetSharedContent()
    {
        try
        {
            if (string.IsNullOrEmpty(MauiProgram.SharedData)) return;

            var asinCode = await AsinUtility.GetAsinCode(MauiProgram.SharedData);
            if (string.IsNullOrEmpty(asinCode)) return;

            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var vm = new ConfirmBookViewModel(asinCode, _bookInfoRepository, _revenueCat, _settingsPreferences);
                await Shell.Current.CurrentPage.ShowPopupAsync(new ConfirmBookView(vm));
            });
        }
        catch (Exception ex)
        {
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.DisplayAlert($"{AppResources.Error}", ex.Message, "OK");
            });
            
        }
        finally
        {
            MauiProgram.SharedData = string.Empty;
        }
    }
}