using System.Globalization;
using Tsundoku.Resources;

namespace Tsundoku.Models;

public class Book
{
    // Muted spine palette; keep in sync with Spine1-5 in Resources/Styles/Colors.xaml.
    private static readonly Color[] _colors =
    [
        Color.FromArgb("#2F4D6E"),
        Color.FromArgb("#5C7F5A"),
        Color.FromArgb("#8E3B3F"),
        Color.FromArgb("#B98A2F"),
        Color.FromArgb("#4F5560"),
    ];

    // Books left unread for this many days are highlighted on the stack screen.
    public const int LongInStackDays = 180;

    public required int Id { get; init; }
    public required DateTime RegistrationDate { get; init; }
    public string RegistrationDateString => RegistrationDate.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture);
    public string StackedOnString => string.Format(AppResources.StackedOn, RegistrationDateString);
    public int DaysInStack => Math.Max(0, (DateTime.Today - RegistrationDate.Date).Days);
    public bool IsLongInStack => DaysInStack >= LongInStackDays;
    public Color Color => _colors[Math.Abs(Id) % _colors.Length];
    public required DateTime ReadDate { get; set; }
    public string ReadDateString => ReadDate.ToString("yyyy.MM.dd", CultureInfo.InvariantCulture);
    public int DaysToFinish => Math.Max(0, (ReadDate.Date - RegistrationDate.Date).Days);
    public string DaysToFinishString => $"{DaysToFinish} {AppResources.Days}";
    public required string Isbn10 { get; init; }
    public string ImageUrl => $"https://images.amazon.com/images/P/{Isbn10}.jpg";
    public string SmallImageUrl => $"https://images.amazon.com/images/P/{Isbn10}.01.TZZZZZZZ";
    public required bool Read { get; set; } = false;
}
