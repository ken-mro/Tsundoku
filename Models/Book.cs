namespace Tsundoku.Models;

public class Book
{
    public required int Id { get; init; }
    public required DateTime RegistrationDate { get; init; }
    public string RegistrationDateString => RegistrationDate.ToString("dd MMM yyyy");
    public required DateTime ReadDate { get; set; }
    public required string Isbn10 { get; init; }
    public string ImageUrl => $"https://images.amazon.com/images/P/{Isbn10}.jpg";
    public required bool Read { get; set; } = false;
}
