using CommunityToolkit.Mvvm.ComponentModel;
using Tsundoku.Repository;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using Tsundoku.Models;

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
    [NotifyPropertyChangedFor(nameof(ReadBookCertificatesCount))]
    ObservableCollection<Book> _readBookCertificates = [];

    public string ReadBookCertificatesCount => ReadBookCertificates.Count.ToString();

    [ObservableProperty]
    bool _isRefreshing = false;

    [RelayCommand]
    public async Task RefreshAsync()
    {
        IsRefreshing = true;
        await Init();
        IsRefreshing = false;
    }

    [RelayCommand]
    private async Task DeleteBookAsync(Book book)
    {
        try
        {
            IsBusy = true;
            var result = await _bookInfoRepository.DeleteBookInfoAsync(book.Id);
            if (result > 0)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Completed", "This book is deleted from the stack.", "OK");
            }
            else
            {
                throw new Exception("Failed to delete this book from the stack.");
            }

            await Init();
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert($"Error", ex.Message, "OK");
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
            ReadBookCertificates = new ObservableCollection<Book>(books.Where(x => x.Read).OrderBy(x => x.Id).ToList());
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}