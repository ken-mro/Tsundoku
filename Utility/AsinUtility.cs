namespace Tsundoku.Utility;

public static class AsinUtility
{
    public static async Task<string> GetAsinCode(string url)
    {
        try
        {
            var redirectUrl = await GetLastUrl(url);
            var asin = ExtractAsinFromUrl(redirectUrl);
            return asin;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    private static string ExtractAsinFromUrl(string url)
    {
        var match = System.Text.RegularExpressions.Regex.Match(url, @"(?:dp|gp/product)/([A-Z0-9]{10})");
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    private static async Task<string> GetLastUrl(string url)
    {
        using HttpClient client = new();
        using HttpResponseMessage response = await client.GetAsync(url);
        response.EnsureSuccessStatusCode();

        return response.RequestMessage?.RequestUri?.ToString() ?? string.Empty;
    }
}
