using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using Tsundoku.Models;
using Tsundoku.Repository;

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
    ObservableCollection<Book> booksInStack;

    [ObservableProperty]
    bool _isRefreshing = false;

    [RelayCommand]
    public async Task RefreshAsync()
    {
        IsRefreshing = true;
        await Init();
        IsRefreshing = false;
    }

    private async Task Init()
    {
        try
        {
            IsBusy = true;
            var books = await _bookInfoRepository.GetAllBooksAsync();
            BooksInStack = new ObservableCollection<Book>(books.Where(x => !x.Read).OrderByDescending(x =>x.Id).ToList());
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
