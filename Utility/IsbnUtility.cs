namespace Tsundoku.Utility;

public static class IsbnUtility
{
    public static string GetIsbn10(string code)
    {
        var fixedCode = code.Replace("-", "").Replace(" ", "");
        if (fixedCode.Length == 10)
        {
            return code;
        }
        else if (fixedCode.Length == 13)
        {
            return ConvertIsbn13ToIsbn10(fixedCode);
        }
        else
        {
            return string.Empty;
        }
    }

    private static string ConvertIsbn13ToIsbn10(string isbn13)
    {
        if (String.IsNullOrEmpty(isbn13))
            throw new ArgumentNullException("isbn13");
        isbn13 = isbn13.Replace("-", "").Replace(" ", "");
        if (isbn13.Length != 13)
            throw new ArgumentException("The ISBN doesn't contain 13 characters.", "isbn13");

        String isbn10 = isbn13.Substring(3, 9);
        int checksum = 0;
        int weight = 10;

        foreach (Char c in isbn10)
        {
            checksum += (int)Char.GetNumericValue(c) * weight;
            weight--;
        }

        checksum = 11 - (checksum % 11);
        if (checksum == 10)
            isbn10 += "X";
        else if (checksum == 11)
            isbn10 += "0";
        else
            isbn10 += checksum;

        return isbn10;
    }
}
