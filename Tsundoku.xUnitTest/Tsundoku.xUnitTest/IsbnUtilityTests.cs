using Tsundoku.Utility;

namespace Tsundoku.xUnitTest
{
    public class IsbnUtilityTests
    {
        [Theory]
        [InlineData("9784101010014", true)]
        [InlineData("978-4-10-101001-4", true)]
        [InlineData("4101010013", true)]
        [InlineData("410101003X", true)]
        [InlineData("abcdefghij", false)]
        [InlineData("978410101001a", false)]
        [InlineData("1234567890123", false)]
        [InlineData("12345", false)]
        [InlineData("", false)]
        public void IsIsbnCode_accepts_isbn10_and_978_979_isbn13(string code, bool expected)
        {
            Assert.Equal(expected, IsbnUtility.IsIsbnCode(code));
        }

        [Theory]
        [InlineData("9784101010014", "4101010013")]
        [InlineData("9784003101018", "4003101014")]
        public void GetIsbn10_converts_isbn13_with_check_digit(string isbn13, string expected)
        {
            Assert.Equal(expected, IsbnUtility.GetIsbn10(isbn13));
        }
    }
}
