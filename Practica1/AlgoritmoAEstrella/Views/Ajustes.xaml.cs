using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace AlgoritmoAEstrella.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    // ReSharper disable once RedundantExtendsListEntry
    public partial class Ajustes : ContentPage
    {
        public Ajustes()
        {
            InitializeComponent();

            if (Device.RuntimePlatform.Equals(Device.iOS))
            {
                Vista.Padding = new Thickness(0, 0, 0, 5);
            }

            if (App.Ajustes.Diagonal && App.Ajustes.Perpendicular)
            {
                Movimiento.SelectedIndex = 2;
            }
            else
            {
                Movimiento.SelectedIndex = App.Ajustes.Diagonal ? 0 : 1;
            }
            
            Movimiento.SelectedIndexChanged += MovimientoOnSelectedIndexChanged;
        }

        private static void MovimientoOnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(sender is Picker picker)) return;
            switch (picker.SelectedIndex)
            {
                case 0:
                    App.Ajustes.Diagonal = true;
                    App.Ajustes.Perpendicular = false;
                    break;
                case 1:
                    App.Ajustes.Perpendicular = true;
                    App.Ajustes.Diagonal = false;
                    break;
                case 2:
                    App.Ajustes.Perpendicular = App.Ajustes.Diagonal = true;
                    break;
            }
        }
    }
}