using AlgoritmoAEstrella.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace AlgoritmoAEstrella.Views
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    // ReSharper disable once RedundantExtendsListEntry
    // ReSharper disable once UnusedType.Global
    public partial class Tablero : ContentPage
    {
        private bool _enEjecucion;
        private bool _terminado;
        private bool _noCambiandoCelda = true;

        private readonly Coordenadas _size = new Coordenadas(10, 10);
        private readonly Baldosa[,] _mapa;

        private Baldosa _inicio;
        private Baldosa _fin;

        public Tablero()
        {
            InitializeComponent();

            if (Device.RuntimePlatform.Equals(Device.iOS))
            {
                Vista.Padding = new Thickness(0, 0, 0, 5);
            }

            TipoBaldosa.ItemsSource = Enum.GetValues(typeof(TiposBaldosa));

            _mapa = new Baldosa[_size.X, _size.Y];
            NuevoTablero();
        }

        private void NuevoTablero()
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Grid.Children.Clear();
                _inicio = null;
                _fin = null;
                _enEjecucion = false;
                _terminado = false;

                for (var i = 0; i < _size.Y; i++)
                {
                    for (var j = 0; j < _size.X; j++)
                    {
                        var coordenadas = new Coordenadas(j, _size.Y - (i + 1));
                        var celda = new Celda()
                        {
                            WidthRequest = 5.0,
                            HeightRequest = 5.0,
                            BackgroundColor = (Color)Application.Current.Resources["AltColor"],
                            Coordenadas = coordenadas,
                            Aspect = Aspect.AspectFit
                        };

                        celda.Clicked += Baldosa_Clicked;

                        _mapa[coordenadas.X, coordenadas.Y] = new Baldosa(coordenadas, celda);

                        Grid.Children.Add(celda, j, i);
                    }
                }
            });
        }

        private async Task AsignarTipoBaldosa(Celda celda)
        {
            switch (TipoBaldosa.SelectedIndex)
            {
                case 0: // Inicio
                    if (_fin != null && celda.Coordenadas.Equals(_fin.Celda.Coordenadas))
                    {
                        MainThread.BeginInvokeOnMainThread(() => { _fin.Celda.Source = ""; });
                        _fin = null;
                    }

                    if (_inicio != null)
                    {
                        MainThread.BeginInvokeOnMainThread(() => { _inicio.Celda.Source = ""; });
                    }

                    MainThread.BeginInvokeOnMainThread(() => { celda.Source = "Inicio"; });

                    _inicio = _mapa[celda.Coordenadas.X, celda.Coordenadas.Y];
                    _mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Accesible = true;
                    _mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Valor = 0;

                    break;

                case 1: // Fin
                    if (_inicio != null && celda.Coordenadas.Equals(_inicio.Celda.Coordenadas))
                    {
                        MainThread.BeginInvokeOnMainThread(() => { _inicio.Celda.Source = ""; });
                        _inicio = null;
                    }

                    if (_fin != null)
                    {
                        MainThread.BeginInvokeOnMainThread(() => { _fin.Celda.Source = ""; });
                    }

                    MainThread.BeginInvokeOnMainThread(() => { celda.Source = "Fin"; });

                    _fin = _mapa[celda.Coordenadas.X, celda.Coordenadas.Y];
                    _mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Accesible = true;
                    _mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Valor = 0;

                    break;

                case 2: // Perro - No accesible
                    if (_inicio != null && celda.Coordenadas.Equals(_inicio.Celda.Coordenadas))
                    {
                        MainThread.BeginInvokeOnMainThread(() => { _inicio.Celda.Source = ""; });
                        _inicio = null;
                    }
                    else if (_fin != null && celda.Coordenadas.Equals(_fin.Celda.Coordenadas))
                    {
                        MainThread.BeginInvokeOnMainThread(() => { _fin.Celda.Source = ""; });
                        _fin = null;
                    }

                    if (_mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Accesible)
                    {
                        MainThread.BeginInvokeOnMainThread(() => { celda.Source = "Perro"; });
                        _mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Accesible = false;
                    }
                    else
                    {
                        MainThread.BeginInvokeOnMainThread(() => { celda.Source = ""; });
                        _mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Accesible = true;
                    }

                    _mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Valor = 0;

                    break;

                case 3: // Montaña
                    if (_inicio != null && celda.Coordenadas.Equals(_inicio.Celda.Coordenadas))
                    {
                        MainThread.BeginInvokeOnMainThread(() => { _inicio.Celda.Source = ""; });
                        _inicio = null;
                    }
                    else if (_fin != null && celda.Coordenadas.Equals(_fin.Celda.Coordenadas))
                    {
                        MainThread.BeginInvokeOnMainThread(() => { _fin.Celda.Source = ""; });
                        _fin = null;
                    }

                    if (_mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Valor == (double)TiposBaldosa.Montaña)
                    {
                        MainThread.BeginInvokeOnMainThread(() => { celda.Source = ""; });
                        _mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Valor = 0;
                    }
                    else
                    {
                        MainThread.BeginInvokeOnMainThread(() => { celda.Source = "Montana"; });
                        _mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Valor = (double)TiposBaldosa.Montaña;
                    }

                    _mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Accesible = true;

                    break;

                case 4: // Río
                    if (_inicio != null && celda.Coordenadas.Equals(_inicio.Celda.Coordenadas))
                    {
                        MainThread.BeginInvokeOnMainThread(() => { _inicio.Celda.Source = ""; });
                        _inicio = null;
                    }
                    else if (_fin != null && celda.Coordenadas.Equals(_fin.Celda.Coordenadas))
                    {
                        MainThread.BeginInvokeOnMainThread(() => { _fin.Celda.Source = ""; });
                        _fin = null;
                    }

                    if (_mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Valor == (double)TiposBaldosa.Rio)
                    {
                        MainThread.BeginInvokeOnMainThread(() => { celda.Source = ""; });
                        _mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Valor = 0;
                    }
                    else
                    {
                        MainThread.BeginInvokeOnMainThread(() => { celda.Source = "Rio"; });
                        _mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Valor = (double)TiposBaldosa.Rio;
                    }

                    _mapa[celda.Coordenadas.X, celda.Coordenadas.Y].Accesible = true;

                    break;

                default:
                    await DisplayAlert("Error",
                        "Tipo no implementado, perdone las molestias",
                        "OK");
                    break;
            }
        }

        private void Play_Clicked(Object sender, EventArgs e)
        {
            if (_inicio == null || _fin == null)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                    await DisplayAlert("Error",
                        "Como mínimo tiene que estar seleccionada la baldosa de inicio y la de fin",
                        "OK")
                );
                return;
            }

            if (!App.Ajustes.Diagonal && !App.Ajustes.Perpendicular)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Error", "No ha seleccionado ningún tipo de movimiento como permitido",
                        "OK");
                });
                return;
            }

            _enEjecucion = true;

            Task.Run(() =>
            {
                var resultado = Algoritmia.Algoritmia.CalculoAEstrella(_inicio, _fin, App.Ajustes.Diagonal,
                    App.Ajustes.Perpendicular, _mapa);
                if (resultado.Coste > 0)
                {
                    PintarCamino(resultado.Camino);
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Shell.Current.DisplayAlert("Info",
                            $"El camino encontrado tiene un coste de {resultado.Coste.ToString(CultureInfo.CurrentCulture)}",
                            "OK");
                    });
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Info", "No se ha encontrado ningún camino", "OK");
                    });
                }
            }).Wait();

            _terminado = true;
            _enEjecucion = false;
        }

        private static void PintarCamino(IReadOnlyCollection<Baldosa> camino)
        {
            foreach (var baldosa in camino)
            {
                if (baldosa.Equals(camino.Last()) || baldosa.Valor > 0)
                {
                    MainThread.BeginInvokeOnMainThread(() => baldosa.Celda.BackgroundColor = Color.Aquamarine);
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        baldosa.Celda.Source = "Inicio";
                        baldosa.Celda.BackgroundColor = Color.Aquamarine;
                    });
                }
            }
        }

        private async void Settings_Clicked(Object sender, EventArgs e)
        {
            if (!_enEjecucion && _noCambiandoCelda)
            {
                await Shell.Current.GoToAsync("settings");
            }
            else
            {
                if (_noCambiandoCelda)
                {
                    await Shell.Current.DisplayAlert("Error",
                        "No se puede entrar en la pagina de ajustes mientras se ejecuta el algoritmo",
                        "OK").ConfigureAwait(false);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error",
                        "Esta cambiando una celda",
                        "OK").ConfigureAwait(false);
                }
            }
        }

        private async void Clear_Clicked(object sender, EventArgs e)
        {
            if (!_enEjecucion && _noCambiandoCelda)
            {
                NuevoTablero();
            }
            else
            {
                if (_noCambiandoCelda)
                {
                    await Shell.Current.DisplayAlert("Error",
                        "No se puede entrar en la pagina de información mientras se ejecuta el algoritmo",
                        "OK").ConfigureAwait(false);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error",
                        "Esta cambiando una celda",
                        "OK").ConfigureAwait(false);
                }
            }
        }

        private async void Info_Clicked(object sender, EventArgs e)
        {
            if (!_enEjecucion && _noCambiandoCelda)
            {
                await Shell.Current.GoToAsync("info");
            }
            else
            {
                if (_noCambiandoCelda)
                {
                    await Shell.Current.DisplayAlert("Error",
                        "No se puede entrar en la pagina de información mientras se ejecuta el algoritmo",
                        "OK").ConfigureAwait(false);
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error",
                        "Esta cambiando una celda",
                        "OK").ConfigureAwait(false);
                }
            }
        }

        private async void Baldosa_Clicked(object sender, EventArgs e)
        {
            if (!_terminado)
            {
                if (!_enEjecucion)
                {
                    if (TipoBaldosa.SelectedIndex != -1)
                    {
                        if (_noCambiandoCelda)
                        {
                            if (!(sender is Celda celda)) return;
                            _noCambiandoCelda = false;
                            await AsignarTipoBaldosa(celda);
                            _noCambiandoCelda = true;
                        }
                        else
                        {
                            await DisplayAlert("Error",
                                "Espere a que se termine de asignar el tipo de otra baldosa anterior antes de cambiar otra",
                                "OK").ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        await DisplayAlert("Error",
                            "Primero selecciona el tipo de baldosa que deseas asignar",
                            "OK").ConfigureAwait(false);
                    }
                }
                else
                {
                    await DisplayAlert("Error",
                        "No se puede cambiar el tipo de baldosa mientras se ejecuta el algoritmo",
                        "OK").ConfigureAwait(false);
                }
            }
            else
            {
                await DisplayAlert("Aviso",
                    "Se esta mostrando un mapa terminado. Debe borrar el antiguo mapa y empezar de nuevo con el botón superior",
                    "OK");
            }
        }
    }
}