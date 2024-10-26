using Tsundoku.Utility;

namespace Tsundoku.xUnitTest
{
    public class AsinUtilityTests
    {
        [Theory]
        [InlineData("https://www.amazon.co.jp/Harry-Potter-Philosophers-Stone-English-ebook/dp/B019PIOJYU/ref=sr_1_5?dib=eyJ2IjoiMSJ9.unjJe6ynanBuiIduQ0xcKHbi_EPJ9vx8FLElMWl_gKhynDWNqUpMMBh7SzR5EFEZcZzm2KIudFwa2HfMHvgUMfIDXd1yiMsd_x1GRfsf1GmqQU8yWIdAH9OLdJGfXhxCbYd_AUSiBv2FnuzLbbJh0wXV-BA7AeSXucFLOj5Rj_XjBBIBZvS5EXmxAhFfgYWQJvnIWiiVzjl8gFtWjHqWSTNZF2MRDhUxS-lhjHbxCnN1VoDtTBTmIqkzoybAnyXhMf0oRr_JoAHva4penhHKoT_mKZtZtcI5Bq13MxyEzMo.SptSehla_z_3AAh7xjbfIr0z-T83B9cLcS5wwH_5cVo&dib_tag=se&keywords=harry+potter&qid=1729690506&sr=8-5", "B019PIOJYU")]
        [InlineData("https://amzn.asia/d/cjTFH6x", "B019PIOJYU")]
        public async Task Get_Asin_code_from_amazon_url(string url, string expected)
        {
            //await IsbnUtility.GetAsinCode(url).Should().Be(expectedResult);
            var asinCode = await AsinUtility.GetAsinCode(url);

            Assert.Equal(expected, asinCode);
        }
    }
}