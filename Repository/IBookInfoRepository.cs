using Tsundoku.Models;

namespace Tsundoku.Repository;

public interface IBookInfoRepository
{
    Task<IEnumerable<BookInfo>> GetBookInfosAsync();
    Task<BookInfo> GetBookInfoAsync(int id);
    Task AddBookInfoAsync(BookInfo bookInfo);
    Task UpdateBookInfoAsync(BookInfo bookInfo);
    Task DeleteBookInfoAsync(int bookInfoId);
}
