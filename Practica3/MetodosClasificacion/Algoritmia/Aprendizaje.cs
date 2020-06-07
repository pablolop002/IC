using System;
using System.Collections.Generic;

namespace MetodosClasificacion.Algoritmia
{
    public abstract class Aprendizaje
    {
        public List<Muestra> Centros { get; set; }
        public List<Clase> Datos { get; set; }
        public List<Muestra> Muestras { get; set; }

        public abstract string Clasificar(Muestra muestraComprobar);

        public abstract void Entrenar();

        protected void CentrosIniciales()
        {
            for (int i = 0; i < Centros.Count; i++)
                foreach (var item in Centros[i].Medidas)
                    Datos[i].Centro.Medidas.Add(item);
        }

        protected double DistanciaCuadrado(Muestra origen, Muestra destino)
        {
            double resultado = 0;

            if (origen.Medidas.Count == destino.Medidas.Count)
            {
                for (int i = 0; i < origen.Medidas.Count; i++)
                {
                    resultado += Math.Pow(origen.Medidas[i] - destino.Medidas[i], 2);
                }

                resultado = Math.Sqrt(resultado);
            }

            return resultado;
        }

        protected double Distancia(Muestra origen, Muestra destino)
        {
            double resultado = 0;

            if (origen.Medidas.Count == destino.Medidas.Count)
            {
                resultado = DistanciaCuadrado(origen, destino);
                resultado = Math.Sqrt(resultado);
            }

            return resultado;
        }
    }
}