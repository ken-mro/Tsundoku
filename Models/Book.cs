namespace Tsundoku.Models;

public class Book
{
    private IEnumerable<Color> _colors = new List<Color>
    {
        Color.FromHex("#E29401"),
        Color.FromHex("#639A2C"),
        Color.FromHex("#BC161C"),
        Color.FromHex("#1D4F77"),
    };

    public required int Id { get; init; }
    public required DateTime RegistrationDate { get; init; }
    public string RegistrationDateString => RegistrationDate.ToString("dd MMM yyyy");
    public Color Color => _colors.ElementAt(Id % 4);
    public required DateTime ReadDate { get; set; }
    public string ReadDateString => ReadDate.ToString("dd MMM yyyy");
    public required string Isbn10 { get; init; }
    public string ImageUrl => $"https://images.amazon.com/images/P/{Isbn10}.jpg";
    public string SmallImageUrl => $"https://images.amazon.com/images/P/{Isbn10}.01.TZZZZZZZ";
    public required bool Read { get; set; } = false;
}
