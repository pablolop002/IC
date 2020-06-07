using System.Collections.Generic;

namespace MetodosClasificacion.Algoritmia
{
    public class Muestra
    {
        public Muestra()
        {
            Medidas = new List<double>();
        }
        public List<double> Medidas { get; set; }
        public string NombreClase { get; set; }

        public override string ToString()
        {
            string resultado = "[";
            if (Medidas == null || Medidas.Count == 0)
                resultado = " ]";
            else
            {
                foreach (var item in Medidas)
                    resultado += item.ToString() + "; ";
                resultado = resultado.Substring(0, resultado.Length - 2) + "]";
            }
            return resultado;
        }

        public static Muestra DuplicarMuestra(Muestra muestra)
        {
            Muestra resultado = new Muestra();
            resultado.NombreClase = muestra.NombreClase;
            foreach (var item in muestra.Medidas)
                resultado.Medidas.Add(item);
            return resultado;
        }

        public Muestra DuplicarMuestra()
        {
            Muestra resultado = new Muestra();
            resultado.NombreClase = this.NombreClase;
            foreach (var item in this.Medidas)
                resultado.Medidas.Add(item);
            return resultado;
        }
    }
}