using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using ID3.Algoritmo;
using ID3.Services;
using Plugin.FilePicker;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace ID3.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    [SuppressMessage("ReSharper", "RedundantExtendsListEntry")]
    [SuppressMessage("ReSharper", "UnusedType.Global")]
    public partial class Main : ContentPage
    {
        private string[] _attributes;
        private string[] _board;
        private DataTable _data;
        private Tree _tree;

        public Main()
        {
            InitializeComponent();
        }

        private async void Board_OnClicked(object sender, EventArgs e)
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
            var board = await CrossFilePicker.Current.PickFile(fileTypes);

            if (board != null)
            {
                _board = ReadLines(board.GetStream, Encoding.Default).ToArray();

                board.Dispose();

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Correcto", "Tabla cargada. Prueba a generar el árbol", "OK");
                    Generate.IsEnabled = true;
                });
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () => await DisplayAlert("Error",
                    "Selección de archivo cancelada", "OK"));
            }
        }

        private async void Attributes_OnClicked(object sender, EventArgs e)
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
            var attributes = await CrossFilePicker.Current.PickFile(fileTypes);

            if (attributes != null)
            {
                _attributes = ReadLines(attributes.GetStream, Encoding.Default).ToArray();

                if (_attributes.Length > 1)
                {
                    MainThread.BeginInvokeOnMainThread(async () => await DisplayAlert("Error",
                        "Los atributos tienen que estar en una única linea separados por comas y sin espacios", "OK"));
                    _attributes = null;
                }
                else
                {
                    var separator = new[] {','};
                    _attributes = _attributes.First().Split(separator, StringSplitOptions.RemoveEmptyEntries);
                }

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Correcto", "Atributos cargados. Selecciona ahora la tabla", "OK");
                    Board.IsEnabled = true;
                });
                attributes.Dispose();
            }
            else
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                    await DisplayAlert("Error", "Selección de archivo cancelada", "OK"));
            }
        }

        private static IEnumerable<string> ReadLines(Func<Stream> streamProvider, Encoding encoding)
        {
            using (var stream = streamProvider())
            using (var reader = new StreamReader(stream, encoding))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        private void Generate_OnClicked(object sender, EventArgs e)
        {
            if (_board == null || _attributes == null)
            {
                MainThread.BeginInvokeOnMainThread(async () =>
                    await DisplayAlert("Error", "", "OK"));
                return;
            }

            _data = TratarTablero();

            if (_data == null) return;
            _tree = Arbol.CreateTreeAndHandleUserOperation(_data);
            var tree = Tree.Print(_tree.Root, _tree.Root.Nombre.ToUpper());
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                Editor.Text = tree;
                Calculate.IsEnabled = true;
                await DisplayAlert("Correcto", "Árbol generado.", "OK");
            });
        }

        private async void Calculate_OnClicked(object sender, EventArgs e)
        {
            var option =
                await DisplayActionSheet("Acción", "Cancelar", null,
                    "Valores de un atributo", "Consulta completa");

            if (option.Equals("Valores de un atributo"))
            {
                var input = await DisplayPromptAsync("Selección de atributo",
                    "Escribe el nombre del atributo del que desee conocer los posibles valores");
                if (_attributes.Any(s => s.Equals(input, StringComparison.OrdinalIgnoreCase)))
                {
                    var column = _attributes.IndexOf(x => x.Equals(input, StringComparison.OrdinalIgnoreCase));
                    MainThread.BeginInvokeOnMainThread(() =>
                        Editor.Text =
                            $"{input}:\n {string.Join(", ", Atributo.ObtenerDiferentesValoresDeColumna(_data, column))}");
                }
                else
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                        await DisplayAlert("Error", "El atributo introducido no es valido", "OK"));
                }
            }
            else if (option.Equals("Consulta completa"))
            {
                try
                {
                    var valuesForQuery = new Dictionary<string, string>();
                    var first = true;
                    var calculationValues = "";

                    foreach (var item in _data.Columns)
                    {
                        if (_data.Columns.IndexOf((DataColumn) item) == _data.Columns.Count - 1) continue;
                        var input = await DisplayPromptAsync("Entrada de datos",
                            $"Introduzca el valor para el campo {item}");
                        valuesForQuery.Add(item.ToString(), input);

                        calculationValues += first ? $"({item}) {input}" : $", ({item}) {input}";
                        first = false;
                    }

                    var resultado = Tree.CalcularResultado(_tree.Root, valuesForQuery, "");

                    if (resultado == "Atributo no encontrado")
                    {
                        MainThread.BeginInvokeOnMainThread(async () =>
                        {
                            Editor.Text = calculationValues;
                            await DisplayAlert("Error",
                                "Se ha introducido una opción no válida y no ha sido posible realizar el cálculo",
                                "OK");
                        });
                    }
                    else
                    {
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            Editor.Text =
                                $"{calculationValues}: {(resultado.Split('>')[resultado.Split('>').Length - 1]).Trim()}";
                        });
                    }
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                        await DisplayAlert("Error", ex.Message, "OK"));
                }
            }
        }

        private DataTable TratarTablero()
        {
            try
            {
                var data = new DataTable();

                foreach (var column in _attributes)
                {
                    data.Columns.Add(new DataColumn(column));
                }

                var columns = data.Columns.Count;

                foreach (var line in _board)
                {
                    var aux = line.Split(',');
                    if (aux.Length != columns)
                    {
                        throw new Exception($"Todas las lineas tienen que tener {columns.ToString()} valores");
                    }

                    var row = data.NewRow();

                    for (var i = 0; i < columns; i++)
                    {
                        if (string.IsNullOrEmpty(aux[i]) || string.IsNullOrWhiteSpace(aux[i]))
                            throw new Exception("No se permiten valores vacíos");

                        row[i] = aux[i];
                    }

                    data.Rows.Add(row);
                }

                if (Atributo.ObtenerDiferentesValoresDeColumna(data, data.Columns.Count - 1).Count > 2)
                {
                    throw new Exception(
                        "La ultima columna es de resultados y solo puede contener dos valores diferentes");
                }

                return data.Rows.Count > 0 ? data : null;
            }
            catch (Exception ex)
            {
                MainThread.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", ex.Message, "OK"));
                return null;
            }
        }

        private void Clear_OnClicked(object sender, EventArgs e)
        {
            _attributes = _board = null;
            _data = null;
            _tree = null;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Board.IsEnabled = Generate.IsEnabled = Calculate.IsEnabled = false;
                Editor.Text = "";
            });
        }
    }
}