using Tsundoku.Models;

namespace Tsundoku.Repository;

public interface IBookInfoRepository
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<int> GetAllBooksCountAsync();
    Task<int> AddBookInfoAsync(string isbn10);
    Task<int> DeleteBookInfoAsync(int id);
    Task<int> UpdateReadStatusAsync(int Id, DateTime readDateTime, bool read);
}
