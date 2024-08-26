using SQLite;
using Tsundoku.Models;

namespace Tsundoku.Repository;

public class BookInfoRepository : IBookInfoRepository
{
    private string _dbPath = Constants.DataBasePath;
    private SQLiteAsyncConnection _conn;

    public BookInfoRepository()
    {
    }

    private async Task Init()
    {
        if (_conn != null)
            return;

        _conn = new SQLiteAsyncConnection(_dbPath);
        await _conn.CreateTableAsync<BookInfo>();
    }

    public Task AddBookInfoAsync(BookInfo bookInfo)
    {
        throw new NotImplementedException();
    }

    public Task DeleteBookInfoAsync(int bookInfoId)
    {
        throw new NotImplementedException();
    }

    public Task<BookInfo> GetBookInfoAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<BookInfo>> GetBookInfosAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateBookInfoAsync(BookInfo bookInfo)
    {
        throw new NotImplementedException();
    }
}
