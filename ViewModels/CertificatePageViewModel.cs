using CommunityToolkit.Mvvm.ComponentModel;
using Tsundoku.Repository;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Tsundoku.Models;
using Tsundoku.Resources;

namespace Tsundoku.ViewModels;

public partial class CertificatePageViewModel : BaseViewModel
{
    private IBookInfoRepository _bookInfoRepository;
    public CertificatePageViewModel(IBookInfoRepository bookInfoRepository)
    {
        _bookInfoRepository = bookInfoRepository;

        _ = Init();
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ReadBookCertificatesCountString))]
    ObservableCollection<Book> _readBookCertificates = [];

    public string ReadBookCertificatesCountString => $"{ReadBookCertificates.Count.ToString()} {AppResources.Certificate_s_}";

    [ObservableProperty]
    bool _isRefreshing = false;

    [ObservableProperty]
    int _gridItemSpan = 1;

    [RelayCommand]
    public async Task RefreshAsync()
    {
        IsRefreshing = true;
        await Init();
        IsRefreshing = false;
    }

    [RelayCommand]
    private async Task DeleteCertificateAsync(Book book)
    {
        try
        {
            IsBusy = true;
            var proceeds = await Shell.Current.CurrentPage.DisplayAlert($"{AppResources.Confirmation}", $"{AppResources.DeleteCert}", $"{AppResources.Yes}", $"{AppResources.No}");
            if (!proceeds) return;

            var result = await _bookInfoRepository.DeleteBookInfoAsync(book.Id);
            if (result <= 0)
            {
                throw new Exception($"{AppResources.FailToDeleteBook}");
            }

            await Init();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert($"{AppResources.Equals}", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
    private async Task Init()
    {
        try
        {
            IsBusy = true;
            var books = await _bookInfoRepository.GetAllBooksAsync();
            ReadBookCertificates = new ObservableCollection<Book>([.. books.Where(x => x.Read).OrderByDescending(x => x.ReadDate)]);
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert($"{AppResources.Error}", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}