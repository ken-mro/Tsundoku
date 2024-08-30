﻿using SQLite;
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

    public async Task<int> AddBookInfoAsync(string isbn10)
    {
        await Init();
        var bookInfo = new BookInfo
        {
            RegistrationDate = DateTime.Now,
            Isbn10 = isbn10,
            Read = false
        };
        return await _conn.InsertAsync(bookInfo);
    }

    public async Task<int> DeleteBookInfoAsync(int id)
    {
        await Init();
        return await _conn.Table<BookInfo>().Where(i => i.Id == id).DeleteAsync();
    }

    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        await Init();
        var bookInfoList = await _conn.Table<BookInfo>().ToListAsync();
        var books = new List<Book>();
        foreach (var bookInfo in bookInfoList)
        {
            books.Add(await GetBook(bookInfo));
        }
        return books;
    }

    public async Task<int> GetAllBooksCountAsync()
    {
        await Init();
        return await _conn.Table<BookInfo>().CountAsync();
    }

    public async Task<int> UpdateReadStatusAsync(int Id, DateTime readDateTime, bool read)
    {
        if (Id == 0) return 0;
        await Init();
        var result = await _conn.ExecuteAsync("UPDATE BookInfo SET Read = ?, ReadDate = ? WHERE Id = ?", read, readDateTime, Id);
        return result;
    }

    private async Task<Book> GetBook(BookInfo bookInfo)
    {
        return await Book.BookFactory(bookInfo.Id, bookInfo.RegistrationDate, bookInfo.ReadDate, bookInfo.Isbn10, bookInfo.Read);
    }
}