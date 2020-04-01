using AlgoritmoAEstrella.Views;
using Xamarin.Forms;

namespace AlgoritmoAEstrella
{
    // ReSharper disable once RedundantExtendsListEntry
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("settings", typeof(Ajustes));
            Routing.RegisterRoute("info", typeof(Info));
        }
    }
}
