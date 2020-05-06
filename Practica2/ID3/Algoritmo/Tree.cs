using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ID3.Algoritmo
{
    public class Tree
    {
        public TreeNode Root { get; set; }

        public static string Print(TreeNode nodo, string result)
        {
            var resultado = "";
            if (nodo?.Hijos == null || nodo.Hijos.Count == 0)
                return $"{result}\n";

            foreach (var hijo in nodo.Hijos)
            {
                resultado += Print(hijo, $"{result} --({hijo.Arista.ToLower()})--> {hijo.Nombre.ToUpper()}");
            }

            return resultado;
        }

        public static string CalcularResultado(TreeNode raiz, IDictionary<string, string> valores, string resultado)
        {
            var encontrado = false;
            resultado += $"{raiz.Nombre.ToUpper()} --";

            if (raiz.EsHoja)
            {
                resultado = $"({raiz.Arista.ToLower()})--> {raiz.Nombre.ToUpper()}";
                encontrado = true;
            }
            else
                foreach (var hijo in raiz.Hijos)
                    foreach (var entrada in valores)
                        if (hijo.Arista.ToUpper().Equals(entrada.Value.ToUpper()) && raiz.Nombre.ToUpper().Equals(entrada.Key.ToUpper()))
                        {
                            valores.Remove(entrada.Key);
                            return $"resultado {CalcularResultado(hijo, valores, $"({hijo.Arista.ToLower()})--> ")}";
                        }

            if (!encontrado)
                resultado = "Atributo no encontrado";

            return resultado;
        }

        public static TreeNode Aprender(DataTable dato, string arista)
        {
            var raiz = CalcularRaiz(dato, arista);

            foreach (var item in raiz.Nodo.AtributosDiferentes)
            {
                var esHoja = CompruebaEsHoja(raiz, dato, item);
                if (!esHoja)
                {
                    var tablaReducida = Operaciones.ReducirTabla(dato, item, raiz.Indice);
                    raiz.Hijos.Add(Aprender(tablaReducida, item));
                }
            }
            return raiz;
        }

        private static bool CompruebaEsHoja(TreeNode raiz, DataTable dato, string atributo)
        {
            var esHoja = true;
            var valoresFinales = new List<string>();

            for (var i = 0; i < dato.Rows.Count; i++)
                if (dato.Rows[i][raiz.Indice].ToString().Equals(atributo))
                    valoresFinales.Add(dato.Rows[i][dato.Columns.Count - 1].ToString());

            if (valoresFinales.Count > 0 && valoresFinales.Any(x => x != valoresFinales[0]))
                esHoja = false;

            if (esHoja)
                raiz.Hijos.Add(new TreeNode(true, valoresFinales[0], atributo));

            return esHoja;
        }

        private static TreeNode CalcularRaiz(DataTable dato, string arista)
        {
            var atributos = new List<Atributo>();
            var indiceMejorMerito = -1;
            var mejorMerito = double.MinValue;

            for (var i = 0; i < dato.Columns.Count - 1; i++)
            {
                var nombresAtributos = Atributo.ObtenerDiferentesValoresDeColumna(dato, i);
                atributos.Add(new Atributo(dato.Columns[i].ToString(), nombresAtributos));
            }

            var entropia = Operaciones.CalcularEntropia(dato);

            for (var i = 0; i < atributos.Count; i++)
            {
                atributos[i].Merito = Operaciones.ObtenerMeritoParaAtributos(dato, i, entropia);
                if (atributos[i].Merito > mejorMerito)
                {
                    mejorMerito = atributos[i].Merito;
                    indiceMejorMerito = i;
                }
            }

            return new TreeNode(atributos[indiceMejorMerito].Nombre, indiceMejorMerito, atributos[indiceMejorMerito], arista);
        }
    }
}