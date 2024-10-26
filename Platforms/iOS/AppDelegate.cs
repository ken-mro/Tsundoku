using Foundation;
using UIKit;

namespace Tsundoku
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override bool OpenUrl(UIApplication application, NSUrl url, NSDictionary options)
        {
            // Capture the shared URL
            string sharedUrl = url.AbsoluteString ?? string.Empty;

            // Store it or navigate to a specific page in the MAUI app
            MauiProgram.SharedData = sharedUrl;

            return true;
        }
    }
}
