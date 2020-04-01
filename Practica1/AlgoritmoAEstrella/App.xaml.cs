using Xamarin.Forms;
using Xamarin.Essentials;

namespace AlgoritmoAEstrella
{
    // ReSharper disable once RedundantExtendsListEntry
    public partial class App : Application
    {
        public static Data.Settings Ajustes { get; private set; }

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            SetTheme();
            base.OnStart();

            Ajustes = new Data.Settings();
        }

        protected override void OnResume()
        {
            SetTheme();
            base.OnResume();
        }

        private static void SetTheme()
        {
            var theme = AppInfo.RequestedTheme;
            switch (theme)
            {
                case AppTheme.Dark:
                    Current.Resources = new Themes.DarkTheme();
                    break;
                default:
                    Current.Resources = new Themes.LightTheme();
                    break;
            }
        }
    }
}
