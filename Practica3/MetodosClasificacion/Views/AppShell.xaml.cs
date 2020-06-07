using Xamarin.Forms;

namespace MetodosClasificacion.Views
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            Routing.RegisterRoute("algoritmos", typeof(Main.Algoritmos));
        }
    }
}
