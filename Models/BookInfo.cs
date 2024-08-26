using SQLite;

namespace Tsundoku.Models;

[Table("BookInfo")]
public class BookInfo
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public DateTime RegistrationDate { get; init; }

    public DateTime ReadDate { get; set; }

    [MaxLength(10)]
    public string Isbn10 { get; init; }

    public bool Read { get; set; }
}
