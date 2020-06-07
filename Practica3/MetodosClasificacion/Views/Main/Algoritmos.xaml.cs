using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using MetodosClasificacion.Algoritmia;
using MetodosClasificacion.Services;
using Plugin.FilePicker;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MetodosClasificacion.Views.Main
{
    [QueryProperty("Activo", "activo")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    public partial class Algoritmos : ContentPage
    {
        // ReSharper disable once UnusedMember.Global
        public string Activo
        {
            set
            {
                switch (Uri.UnescapeDataString(value))
                {
                    case "1":
                        TitleBayes.IsVisible = ButtonBayes.IsVisible = BayesResultado.IsVisible = true;
                        break;
                    case "2":
                        TitleLloyd.IsVisible = ButtonLloyd.IsVisible = LloydResultado.IsVisible = true;
                        break;
                    case "3":
                        TitleKmedias.IsVisible = ButtonKmedias.IsVisible = KmediasResultado.IsVisible = true;
                        break;
                    case "4":
                        ButtonBayes.IsVisible = ButtonLloyd.IsVisible = ButtonKmedias.IsVisible =
                            KmediasResultado.IsVisible = LloydResultado.IsVisible = BayesResultado.IsVisible =
                                TitleKmedias.IsVisible = TitleLloyd.IsVisible = TitleBayes.IsVisible = true;
                        break;
                }
            }
        }

        private string[] _muestra;

        public Algoritmos()
        {
            InitializeComponent();
        }

        private void KmediasComprobar_Clicked(object sender, EventArgs e)
        {
            try
            {
                var muestraComprobar = new Muestra()
                {
                    Medidas = new List<double>()
                    {
                        double.Parse(Muestra1.Text.Replace('.', ',')),
                        double.Parse(Muestra2.Text.Replace('.', ',')),
                        double.Parse(Muestra3.Text.Replace('.', ',')),
                        double.Parse(Muestra4.Text.Replace('.', ','))
                    }
                };

                MainThread.BeginInvokeOnMainThread(() =>
                    KmediasResultado.Text = App.Kmedias.Clasificar(muestraComprobar));
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                    DisplayAlert("Error", ex.Message, "OK"));
            }
        }

        private void BayesComprobar_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var muestraComprobar = new Muestra()
                {
                    Medidas = new List<double>()
                    {
                        double.Parse(Muestra1.Text.Replace('.', ',')),
                        double.Parse(Muestra2.Text.Replace('.', ',')),
                        double.Parse(Muestra3.Text.Replace('.', ',')),
                        double.Parse(Muestra4.Text.Replace('.', ','))
                    }
                };

                MainThread.BeginInvokeOnMainThread(() =>
                    BayesResultado.Text = App.Bayes.Clasificar(muestraComprobar));
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                    DisplayAlert("Error", ex.Message, "OK"));
            }
        }

        private void LloydComprobar_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var muestraComprobar = new Muestra()
                {
                    Medidas = new List<double>()
                    {
                        double.Parse(Muestra1.Text.Replace('.', ',')),
                        double.Parse(Muestra2.Text.Replace('.', ',')),
                        double.Parse(Muestra3.Text.Replace('.', ',')),
                        double.Parse(Muestra4.Text.Replace('.', ','))
                    }
                };

                MainThread.BeginInvokeOnMainThread(() =>
                    LloydResultado.Text = App.Lloyd.Clasificar(muestraComprobar));
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                    DisplayAlert("Error", ex.Message, "OK"));
            }
        }

        private async void Muestra_OnClicked(object sender, EventArgs e)
        {
            if (Device.RuntimePlatform.Equals(DevicePlatform.Android.ToString()))
            {
                var read = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
                if (read != PermissionStatus.Granted)
                {
                    read = await Permissions.RequestAsync<Permissions.StorageRead>();

                    if (read != PermissionStatus.Granted)
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                            await DisplayAlert("Error", "No ha concedido los permisos necesarios", "OK"));
                        return;
                    }
                }
            }

            var fileTypes = DependencyService.Get<ISaveDocs>().GetFileTypes();
            var muestra = await CrossFilePicker.Current.PickFile(fileTypes);

            if (muestra != null)
            {
                _muestra = ReadLines(muestra.GetStream, Encoding.Default).ToArray();

                muestra.Dispose();

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Correcto", "Muestra cargada", "OK");
                });
                
                LeerMuestra();
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () => await DisplayAlert("Error",
                    "Selección de archivo cancelada", "OK"));
            }
        }

        private static IEnumerable<string> ReadLines(Func<Stream> streamProvider, Encoding encoding)
        {
            using var stream = streamProvider();
            using var reader = new StreamReader(stream, encoding);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
            }
        }

        private void LeerMuestra()
        {
            var read = new Muestra();
            try
            {
                var values = _muestra[0].Split(',');

                for (var i = 0; i < values.Length - 1; i++)
                {
                    read.Medidas.Add(double.Parse(values[i].Replace('.', ',')));
                }

                read.NombreClase = values[values.Length - 1];

                if (read.Medidas.Count != 0)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Muestra1.Text = read.Medidas[0].ToString(CultureInfo.InvariantCulture);
                        Muestra2.Text = read.Medidas[1].ToString(CultureInfo.InvariantCulture);
                        Muestra3.Text = read.Medidas[2].ToString(CultureInfo.InvariantCulture);
                        Muestra4.Text = read.Medidas[3].ToString(CultureInfo.InvariantCulture);
                        MuestraClase.Text = read.NombreClase.ToString(CultureInfo.InvariantCulture);
                    });
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                        DisplayAlert("Error", "El fichero de muestra no tiene los valores separados por comas", "OK"));
                }
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                    DisplayAlert("Error", ex.Message, "OK"));
            }
        }

        private void Muestra_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if(e.OldTextValue == null || e.OldTextValue.Equals(e.NewTextValue)) return;
            
            MainThread.BeginInvokeOnMainThread(() =>
            {
                MuestraClase.Text = " ";
                BayesResultado.Text = " ";
                LloydResultado.Text = " ";
                KmediasResultado.Text = " ";
            });
        }
    }
}