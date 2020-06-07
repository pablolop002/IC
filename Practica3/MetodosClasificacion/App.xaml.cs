using System.Collections.Generic;
using MetodosClasificacion.Themes;
using MetodosClasificacion.Views;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace MetodosClasificacion
{
    public partial class App : Application
    {
        private static AppTheme _theme = AppTheme.Light;
        
        public static List<Algoritmia.Clase> Data { get; set; }
        public static List<Algoritmia.Muestra> Muestras { get; set; }
        public static Algoritmia.Kmedias Kmedias { get; set; }
        public static Algoritmia.Bayes Bayes { get; set; }
        public static Algoritmia.Lloyd Lloyd { get; set; }

        public App()
        {
            Current.On<Xamarin.Forms.PlatformConfiguration.Android>()
                .UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize);

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
