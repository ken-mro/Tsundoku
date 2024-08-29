

using SkiaSharp;

namespace Tsundoku.Models;

public class Book
{
    public Book(int id, DateTime registrationDate, DateTime readDate, string isbn10, bool read)
    {
        Id = id;
        RegistrationDate = registrationDate;
        ReadDate = readDate;
        Isbn10 = isbn10;
        Read = read;
        _ = LoadImageAsync(ImageUrl);
    }

    private IEnumerable<Color> _colors = new List<Color>
    {
        Color.FromHex("#E29401"),
        Color.FromHex("#639A2C"),
        Color.FromHex("#BC161C"),
        Color.FromHex("#1D4F77"),
    };

    public int Id { get; init; }
    public DateTime RegistrationDate { get; init; }
    public string RegistrationDateString => RegistrationDate.ToString("dd MMM yyyy");
    public Color Color => _colors.ElementAt(Id % 4);
    public DateTime ReadDate { get; set; }
    public string ReadDateString => ReadDate.ToString("dd MMM yyyy");
    public string Isbn10 { get; init; }
    public string ImageUrl => $"https://images.amazon.com/images/P/{Isbn10}.jpg";
    public ImageSource ImageSource { get; private set; }
    public bool Read { get; set; } = false;

    public async Task LoadImageAsync(string imageUrl)
    {
        var resizedImage = await DownloadAndResizeImageAsync(imageUrl, 150, 150);
        var stream = ConvertBitmapToStream(resizedImage);
        ImageSource = ImageSource.FromStream(() => stream);
    }

    public static async Task<SKBitmap> DownloadAndResizeImageAsync(string imageUrl, int width, int height)
    {
        using (HttpClient client = new HttpClient())
        {
            var imageData = await client.GetByteArrayAsync(imageUrl);
            using (var inputStream = new SKMemoryStream(imageData))
            {
                var original = SKBitmap.Decode(inputStream);
                var resized = original.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);
                return resized;
            }
        }
    }

    public static Stream ConvertBitmapToStream(SKBitmap bitmap)
    {
        using (var image = SKImage.FromBitmap(bitmap))
        using (var data = image.Encode(SKEncodedImageFormat.Jpeg, 75))
        {
            return new MemoryStream(data.ToArray());
        }
    }
}
