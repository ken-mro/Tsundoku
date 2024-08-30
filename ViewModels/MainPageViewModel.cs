using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Tsundoku.Models;
using Tsundoku.Repository;
using Tsundoku.Resources;

namespace Tsundoku.ViewModels;

public partial class MainPageViewModel : BaseViewModel
{
    private IBookInfoRepository _bookInfoRepository;
    public MainPageViewModel(IBookInfoRepository bookInfoRepository)
    {
        _bookInfoRepository = bookInfoRepository;

        _ = Init();
    }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(BooksInStackCountString))]
    ObservableCollection<Book> _booksInStack = [];

    public string BooksInStackCountString => $"{BooksInStack.Count.ToString()} {AppResources.BooksInStack}";

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

            var proceeds = await Shell.Current.CurrentPage.DisplayAlert($"{AppResources.Confirmation}", $"{AppResources.DeleteBook}", $"{AppResources.Yes}", $"{AppResources.No}");
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
            await Shell.Current.DisplayAlert($"{AppResources.Error}", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ReadBookAsync(Book book)
    {
        try
        {
            IsBusy = true;

            var result = await _bookInfoRepository.UpdateReadStatusAsync(book.Id, DateTime.Now, true);
            if (result <= 0)
            {
                throw new Exception($"{AppResources.FailToIssueCert}");
            }
            await Init();
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

    private async Task Init()
    {
        try
        {
            IsBusy = true;
            var books = await _bookInfoRepository.GetAllBooksAsync();
            BooksInStack = new ObservableCollection<Book>([..books.Where(x => !x.Read).OrderByDescending(x =>x.RegistrationDate)]);
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
