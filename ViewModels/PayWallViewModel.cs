using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Maui.RevenueCat.InAppBilling.Enums;
using Maui.RevenueCat.InAppBilling.Models;
using Maui.RevenueCat.InAppBilling.Services;
using System.Collections.ObjectModel;
using Tsundoku.Repository;
using Tsundoku.Resources;

namespace Tsundoku.ViewModels;

public partial class PayWallViewModel : BaseViewModel
{
    private readonly IRevenueCatBilling _revenueCatBilling;
    private readonly SettingsPreferences _settingsPreferences;
    public Popup Popup = default!;

    //RC data
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AreOfferingsLoaded))]
    private ObservableCollection<OfferingDto> _loadedOfferings = [];

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(MonthlyButtonText))]
    private PackageDto _monthlySubscription = new();

    //UI data
    public bool AreOfferingsLoaded => LoadedOfferings.Any();
    public string MonthlyButtonText => $"{AppResources.MonthlySubFor} {MonthlySubscription.Product.Pricing.PriceLocalized}";

    public PayWallViewModel(IRevenueCatBilling revenueCatBilling, SettingsPreferences settingsPreferences)
    {
        _revenueCatBilling = revenueCatBilling;
        _settingsPreferences = settingsPreferences;
        Title = "Pay Wall";
    }

    [RelayCommand]
    private async Task LoadOfferingsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var loadedOfferings = await _revenueCatBilling.GetOfferings();
            var monthlySubscription = loadedOfferings
                .SelectMany(x => x.AvailablePackages)
                .FirstOrDefault(x => x.Identifier == DefaultPackageIdentifier.Monthly);

            if (monthlySubscription is null)
            {
                await Shell.Current.CurrentPage.DisplayAlertAsync(AppResources.Error, AppResources.FailToLoadOfferings, "OK");
                return;
            }

            MonthlySubscription = monthlySubscription;
            LoadedOfferings = new ObservableCollection<OfferingDto>(loadedOfferings);
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

    [RelayCommand]
    private async Task BuyItemAsync(PackageDto packageDto)
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var purchaseResult = await _revenueCatBilling.PurchaseProduct(packageDto);
            _settingsPreferences.SetIsSubscribed(purchaseResult.IsSuccess);
            if (purchaseResult.IsSuccess)
            {
                await Popup.CloseAsync();
            }
            else if (purchaseResult.ErrorStatus is not PurchaseErrorStatus.PurchaseCancelledError)
            {
                await Shell.Current.CurrentPage.DisplayAlertAsync(AppResources.Error, purchaseResult.ErrorStatus?.ToString() ?? AppResources.FailToLoadOfferings, "OK");
            }
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
