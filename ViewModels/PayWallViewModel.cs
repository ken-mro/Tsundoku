using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public Popup Popup;

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
    private void LoadOfferings()
    {
        Task.Run(async () =>
        {
            var loadedOfferings = await _revenueCatBilling.GetOfferings();
            LoadedOfferings = new ObservableCollection<OfferingDto>(loadedOfferings);

            MonthlySubscription = LoadedOfferings
                .SelectMany(x => x.AvailablePackages)
                .First(x => x.Identifier == DefaultPackageIdentifier.Monthly);
        });
    }

    [RelayCommand]
    private void BuyItem(PackageDto packageDto)
    {
        if (IsBusy) return;
        IsBusy = true;

        Task.Run(async () =>
        {
            var purchaseResult = await _revenueCatBilling.PurchaseProduct(packageDto);
            _settingsPreferences.SetIsSubscribed(purchaseResult.IsSuccess);
            IsBusy = false;
            if (purchaseResult.IsSuccess)
            {
                await Popup.CloseAsync();
            }
        });
    }
}