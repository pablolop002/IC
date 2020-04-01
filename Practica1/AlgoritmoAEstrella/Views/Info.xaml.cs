using AlgoritmoAEstrella.Data;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AlgoritmoAEstrella.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    // ReSharper disable once RedundantExtendsListEntry
    public partial class Info : ContentPage
    {
        public Info()
        {
            InitializeComponent();
            MonTitle.Text = $"Montaña (Coste: {((int)TiposBaldosa.Montaña).ToString()})";
            RioTitle.Text = $"Rio (Coste: {((int)TiposBaldosa.Rio).ToString()})";
        }
    }
}