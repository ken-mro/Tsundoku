using Android.App;
using Android.Content.PM;
using Android.Content;
using Android.OS;
using System.Text.RegularExpressions;

namespace Tsundoku
{
    [Activity(Theme = "@style/Maui.SplashTheme", Exported = true, MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [IntentFilter([Intent.ActionSend], Categories = [Intent.CategoryDefault], DataMimeType = "text/plain")] // Add ****
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            TryToSaveSharedUrl(Intent!);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
            TryToSaveSharedUrl(intent);
        }

        private static void TryToSaveSharedUrl(Intent intent)
        {
            if (intent?.Action == Intent.ActionSend && intent.Type == "text/plain")
            {
                string sharedText = intent.GetStringExtra(Intent.ExtraText) ?? string.Empty;
                if (string.IsNullOrEmpty(sharedText)) return;

                string urlPattern = @"https?://[^\s]+";
                Match match = Regex.Match(sharedText, urlPattern);
                if (!match.Success) return;

                string url = match.Value;
                MauiProgram.SharedData = url;
            }
        }
    }
}
