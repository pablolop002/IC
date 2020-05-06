using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ID3.Algoritmo
{
    public class Operaciones
    {
        public static void Merito(DataTable datos, string resultados)
        {
            var datosTabla = new Dictionary<DataColumn, int>();

            var columnaResultados = datos.Columns[resultados];

            foreach (DataColumn columna in datos.Columns)
            {
                var datosColumna = new Dictionary<string, Dictionary<string, int>>();

                if (columna.ColumnName != resultados)
                {
                    foreach (DataRow fila in datos.Rows)
                    {
                        var valor = fila[columna].ToString();
                        var resultadoValor = fila[columnaResultados].ToString();

                        if (datosColumna.ContainsKey(valor))
                            if (datosColumna[valor].ContainsKey(resultadoValor))
                                datosColumna[valor][resultadoValor]++;
                            else
                                datosColumna[valor] = new Dictionary<string, int>() { { resultadoValor, 1 } };
                        else
                        {
                            datosColumna.Add(valor, new Dictionary<string, int>() { { resultadoValor, 1 } });
                        }
                    }
                }
                var merito = 0;
                foreach (var item in datosColumna)
                {

                }

                datosTabla.Add(columna, merito);
            }
        }

        internal static DataTable ReducirTabla(DataTable dato, string arista, int raizTabla)
        {
            var tabla = new DataTable();

            for (var i = 0; i < dato.Columns.Count; i++)
                tabla.Columns.Add(dato.Columns[i].ToString());

            for (var i = 0; i < dato.Rows.Count; i++)
                if (dato.Rows[i][raizTabla].ToString().Equals(arista))
                {
                    var row = new string[dato.Columns.Count];

                    for (var j = 0; j < dato.Columns.Count; j++)
                        row[j] = dato.Rows[i][j].ToString();

                    tabla.Rows.Add(row);
                }

            tabla.Columns.Remove(tabla.Columns[raizTabla]);

            return tabla;
        }

        internal static double CalcularEntropia(DataTable data)
        {
            var filas = data.Rows.Count;
            var cantidadDeValoresDiferentes = CantidadDeAristasYResultadosPositivos(data, data.Columns.Count - 1);

            var pasos = cantidadDeValoresDiferentes
                .Select(item => item[0, 0] / (double)filas)
                .Select(division => -division * Math.Log(division, 2))
                .ToList();

            return pasos.Sum();
        }

        internal static double ObtenerMeritoParaAtributos(DataTable dato, int colIndex, double entropiaTabla)
        {
            var numFilas = dato.Rows.Count;
            var cantidadDeValoresDiferentes = CantidadDeAristasYResultadosPositivos(dato, colIndex);
            var pasos = new List<double>();

            foreach (var item in cantidadDeValoresDiferentes)
            {
                var firstDivision = item[0, 1] / (double)item[0, 0];
                var secondDivision = (item[0, 0] - item[0, 1]) / (double)item[0, 0];

                if (firstDivision == 0 || secondDivision == 0)
                    pasos.Add(0.0);
                else
                    pasos.Add(-firstDivision * Math.Log(firstDivision, 2) - secondDivision * Math.Log(secondDivision, 2));
            }

            var merito = pasos.Select((t, i) => cantidadDeValoresDiferentes[i][0, 0] / (double)numFilas * t).Sum();
            merito = entropiaTabla - merito;
            return merito;
        }

        internal static List<int[,]> CantidadDeAristasYResultadosPositivos(DataTable dato, int indiceColumna)
        {
            var encontrados = new List<int[,]>();
            var conocidos = CuentaValoresConocidos(dato, indiceColumna);

            foreach (var item in conocidos)
            {
                var cantidad = 0;
                var cantidadPositiva = 0;

                for (var i = 0; i < dato.Rows.Count; i++)
                    if (dato.Rows[i][indiceColumna].ToString().Equals(item))
                    {
                        cantidad++;

                        if (dato.Rows[i][dato.Columns.Count - 1].ToString().Equals(dato.Rows[0][dato.Columns.Count - 1]))
                            cantidadPositiva++;
                    }

                int[,] array = { { cantidad, cantidadPositiva } };
                encontrados.Add(array);
            }

            return encontrados;
        }

        internal static IEnumerable<string> CuentaValoresConocidos(DataTable dato, int indiceColumna)
        {
            var valoresConocidos = new List<string>();

            if (dato.Rows.Count > 0)
                valoresConocidos.Add(dato.Rows[0][indiceColumna].ToString());

            for (var j = 1; j < dato.Rows.Count; j++)
                if (valoresConocidos.All(item => !dato.Rows[j][indiceColumna].ToString().Equals(item)))
                    valoresConocidos.Add(dato.Rows[j][indiceColumna].ToString());

            return valoresConocidos;
        }
    }
}