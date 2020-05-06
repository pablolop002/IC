using System.Diagnostics.CodeAnalysis;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ID3.Themes
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class LightTheme : ResourceDictionary
    {
        public LightTheme()
        {
            InitializeComponent();
        }
    }
}