using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    // ReSharper disable once RedundantExtendsListEntry
    public partial class Loader : ContentPage
    {
        private string[] _muestra;

        public Loader()
        {
            InitializeComponent();
        }

        private async void Clase_OnClicked(object sender, EventArgs e)
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

                MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Correcto", "Muestra cargada", "OK"));

                TratarClase();
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () => await DisplayAlert("Error",
                    "Selección de archivo cancelada", "OK"));
            }
        }

        private void TratarClase()
        {
            var numReadElements = 0;
            App.Data = new List<Clase>();
            App.Muestras = new List<Muestra>();
            string nombreClase;
            var fila = 0;

            try
            {
                foreach (var line in _muestra)
                {
                    var values = line.Split(',');

                    ++fila;

                    var read = new Muestra();
                    nombreClase = "";

                    if (numReadElements == 0)
                    {
                        numReadElements = values.Length;
                    }

                    if (numReadElements != values.Length)
                    {
                        throw new Exception($"La fila {fila} tiene un numero de datos diferente al resto.");
                    }

                    for (var i = 0; i < numReadElements; i++)
                    {
                        if (i != numReadElements - 1)
                            read.Medidas.Add(float.Parse(values[i].Replace('.', ',')));
                        else
                        {
                            read.NombreClase = values[i];
                            nombreClase = values[i];
                        }
                    }

                    var encontrado = App.Data.FirstOrDefault(dato => dato.Nombre == nombreClase);

                    if (encontrado == null)
                        App.Data.Add(new Clase()
                        {
                            Nombre = nombreClase,
                        });
                    App.Muestras.Add(read);
                }

                MainThread.BeginInvokeOnMainThread(() =>
                    DisplayAlert("Correcto", "Clases procesadas correctamente", "OK"));
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Error", ex.Message, "OK"));
                App.Data = null;
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

        private List<Clase> CopiarDatos(List<Clase> data)
        {
            var resultado = new List<Clase>();

            foreach (var item in data)
            {
                var nueva = new Clase {Nombre = item.Nombre, Centro = new Muestra()};
                foreach (var item2 in item.Centro.Medidas)
                {
                    nueva.Centro.Medidas.Add(item2);
                }

                resultado.Add(nueva);
            }

            return resultado;
        }

        private bool EntrenarBayes()
        {
            try
            {
                App.Bayes = new Bayes()
                {
                    Centros = new List<Muestra>()
                    {
                        new Muestra()
                        {
                            Medidas = new List<double>()
                            {
                                double.Parse(Clase1Centro1.Text.Replace('.', ',')),
                                double.Parse(Clase1Centro2.Text.Replace('.', ',')),
                                double.Parse(Clase1Centro3.Text.Replace('.', ',')),
                                double.Parse(Clase1Centro4.Text.Replace('.', ','))
                            }
                        },
                        new Muestra()
                        {
                            Medidas = new List<double>()
                            {
                                double.Parse(Clase2Centro1.Text.Replace('.', ',')),
                                double.Parse(Clase2Centro2.Text.Replace('.', ',')),
                                double.Parse(Clase2Centro3.Text.Replace('.', ',')),
                                double.Parse(Clase2Centro4.Text.Replace('.', ','))
                            }
                        }
                    },
                    Datos = CopiarDatos(App.Data),
                    Muestras = App.Muestras
                };
                App.Bayes.Entrenar();
                return true;
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                    DisplayAlert("Error", ex.Message, "OK"));
                return false;
            }
        }

        private void EntrenarBayes_OnClicked(object sender, EventArgs e)
        {
            if (EntrenarBayes())
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        await Shell.Current.GoToAsync("algoritmos?activo=1");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", ex.Message, "OK");
                    }
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Error", "Error al entrenar el algoritmo", "OK"));
            }
        }

        private bool EntrenarLloyd()
        {
            try
            {
                App.Lloyd = new Lloyd()
                {
                    Tolerancia = float.Parse(LloydTolerancia.Text.Replace('.', ',')),
                    Iteraciones = int.Parse(LloydIteraciones.Text),
                    Aprendizaje = float.Parse(LloydAprendizaje.Text.Replace('.', ',')),
                    Centros = new List<Muestra>()
                    {
                        new Muestra()
                        {
                            Medidas = new List<double>()
                            {
                                double.Parse(Clase1Centro1.Text.Replace('.', ',')),
                                double.Parse(Clase1Centro2.Text.Replace('.', ',')),
                                double.Parse(Clase1Centro3.Text.Replace('.', ',')),
                                double.Parse(Clase1Centro4.Text.Replace('.', ','))
                            }
                        },
                        new Muestra()
                        {
                            Medidas = new List<double>()
                            {
                                double.Parse(Clase2Centro1.Text.Replace('.', ',')),
                                double.Parse(Clase2Centro2.Text.Replace('.', ',')),
                                double.Parse(Clase2Centro3.Text.Replace('.', ',')),
                                double.Parse(Clase2Centro4.Text.Replace('.', ','))
                            }
                        }
                    },
                    Datos = CopiarDatos(App.Data),
                    Muestras = App.Muestras
                };
                App.Lloyd.Entrenar();
                return true;
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                    DisplayAlert("Error", ex.Message, "OK"));
                return false;
            }
        }

        private void EntrenarLloyd_OnClicked(object sender, EventArgs e)
        {
            if (EntrenarLloyd())
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        await Shell.Current.GoToAsync("algoritmos?activo=2");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", ex.Message, "OK");
                    }
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Error", "Error al entrenar el algoritmo", "OK"));
            }
        }

        private bool EntrenarKmedias()
        {
            try
            {
                App.Kmedias = new Kmedias()
                {
                    Tolerancia = float.Parse(KmediasTolerancia.Text.Replace('.', ',')),
                    PesoExponencialB = float.Parse(Kmediasb.Text.Replace('.', ',')),
                    Centros = new List<Muestra>()
                    {
                        new Muestra()
                        {
                            Medidas = new List<double>()
                            {
                                double.Parse(Clase1Centro1.Text.Replace('.', ',')),
                                double.Parse(Clase1Centro2.Text.Replace('.', ',')),
                                double.Parse(Clase1Centro3.Text.Replace('.', ',')),
                                double.Parse(Clase1Centro4.Text.Replace('.', ','))
                            }
                        },
                        new Muestra()
                        {
                            Medidas = new List<double>()
                            {
                                double.Parse(Clase2Centro1.Text.Replace('.', ',')),
                                double.Parse(Clase2Centro2.Text.Replace('.', ',')),
                                double.Parse(Clase2Centro3.Text.Replace('.', ',')),
                                double.Parse(Clase2Centro4.Text.Replace('.', ','))
                            }
                        }
                    },
                    Datos = CopiarDatos(App.Data),
                    Muestras = App.Muestras
                };
                App.Kmedias.Entrenar();
                return true;
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                    DisplayAlert("Error", ex.Message, "OK"));
                return false;
            }
        }

        private void EntrenarKmedias_OnClicked(object sender, EventArgs e)
        {
            if (EntrenarKmedias())
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        await Shell.Current.GoToAsync("algoritmos?activo=3");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", ex.Message, "OK");
                    }
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() => DisplayAlert("Error", "Error al entrenar el algoritmo", "OK"));
            }
        }

        private void EntrenarTodos_OnClicked(object sender, EventArgs e)
        {
            if (EntrenarBayes() && EntrenarKmedias() && EntrenarLloyd())
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    try
                    {
                        await Shell.Current.GoToAsync("algoritmos?activo=4");
                    }
                    catch (Exception ex)
                    {
                        await DisplayAlert("Error", ex.Message, "OK");
                    }
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(() =>
                    DisplayAlert("Error", "Error al entrenar alguno de los algoritmos", "OK"));
            }
        }
    }
}