using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Tsundoku.Models;
using Tsundoku.Repository;

namespace Tsundoku.ViewModels;

public partial class ConfirmBookViewModel : BaseViewModel
{
    private IBookInfoRepository _bookInfoRepository;
	public ConfirmBookViewModel(string isbn10, IBookInfoRepository bookInfoRepository)
	{
        _bookInfoRepository = bookInfoRepository;
        _isbn10 = isbn10;
        ImageUrl = $"https://images.amazon.com/images/P/{_isbn10}.jpg";
    }

    [ObservableProperty]
    string _imageUrl = string.Empty;

	string _isbn10;

    public Popup Popup;

    [RelayCommand]
    async Task StackBookAsync()
    {
        if (IsBusy)
            return;
        try
        {
            IsBusy = true;
            var result = await _bookInfoRepository.AddBookInfoAsync(_isbn10);
            if (result > 0)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Completed", "The book stacked successfully", "OK");
            }
            else
            {
                throw new Exception("Failed to stack the book");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.CurrentPage.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
            await Popup.CloseAsync();
        }
    }
}