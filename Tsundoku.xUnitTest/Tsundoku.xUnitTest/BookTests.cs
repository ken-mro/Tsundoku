using Tsundoku.Models;

namespace Tsundoku.xUnitTest
{
    public class BookTests
    {
        private static Book CreateBook(int id = 1, DateTime? registered = null, DateTime? read = null) => new()
        {
            Id = id,
            RegistrationDate = registered ?? DateTime.Today,
            ReadDate = read ?? DateTime.Today,
            Isbn10 = "4101010013",
            Read = read is not null,
        };

        [Fact]
        public void DaysInStack_counts_whole_days_since_registration()
        {
            var book = CreateBook(registered: DateTime.Today.AddDays(-12).AddHours(15));

            Assert.Equal(12, book.DaysInStack);
        }

        [Fact]
        public void DaysInStack_is_never_negative()
        {
            var book = CreateBook(registered: DateTime.Today.AddDays(3));

            Assert.Equal(0, book.DaysInStack);
        }

        [Theory]
        [InlineData(179, false)]
        [InlineData(180, true)]
        public void IsLongInStack_starts_at_threshold(int days, bool expected)
        {
            var book = CreateBook(registered: DateTime.Today.AddDays(-days));

            Assert.Equal(expected, book.IsLongInStack);
        }

        [Fact]
        public void DaysToFinish_is_days_between_registration_and_read()
        {
            var book = CreateBook(registered: new DateTime(2026, 1, 10, 23, 0, 0), read: new DateTime(2026, 3, 1, 1, 0, 0));

            Assert.Equal(50, book.DaysToFinish);
        }

        [Fact]
        public void Dates_use_invariant_dotted_format()
        {
            var book = CreateBook(registered: new DateTime(2026, 3, 9), read: new DateTime(2026, 10, 4));

            Assert.Equal("2026.03.09", book.RegistrationDateString);
            Assert.Equal("2026.10.04", book.ReadDateString);
        }

        [Fact]
        public void Color_cycles_through_spine_palette()
        {
            var colors = Enumerable.Range(0, 5).Select(i => CreateBook(id: i).Color.ToArgbHex()).ToList();

            Assert.Equal(5, colors.Distinct().Count());
            Assert.Equal(CreateBook(id: 0).Color.ToArgbHex(), CreateBook(id: 5).Color.ToArgbHex());
        }
    }
}
