using Xamarin.Forms;
using ID3.Themes;
using Xamarin.Essentials;

namespace ID3
{
    public partial class App : Application
    {
        private static AppTheme _theme = AppTheme.Light;

        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            SetTheme();
            base.OnStart();
        }

        protected override void OnResume()
        {
            SetTheme();
            base.OnResume();
        }

        private static void SetTheme()
        {
            _theme = AppInfo.RequestedTheme;

            switch (_theme)
            {
                case AppTheme.Dark:
                    Current.Resources = new DarkTheme();
                    break;
                default:
                    Current.Resources = new LightTheme();
                    break;
            }
        }
    }
}
