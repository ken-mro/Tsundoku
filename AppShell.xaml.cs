using Tsundoku.Views;

namespace Tsundoku
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(CameraPageView), typeof(CameraPageView));
        }
    }
}
