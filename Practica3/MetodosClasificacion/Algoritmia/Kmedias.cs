using System;
using System.Collections.Generic;
using System.Linq;

namespace MetodosClasificacion.Algoritmia
{
    public class Kmedias : Aprendizaje
    {
        private int iteraciones = 0;

        public double Tolerancia { get; set; }
        public double PesoExponencialB { get; set; }

        public List<double> CalcularPertenenciaClases(Muestra muestraComprobar)
        {
            List<double> distancias = new List<double>();
            foreach (Muestra item in Centros)
                distancias.Add(this.DistanciaCuadrado(item, muestraComprobar));

            for (int i = 0; i < distancias.Count; i++)
                distancias[i] = Math.Pow(1 / distancias[i], (1 / (PesoExponencialB - 1)));

            double suma = distancias.Sum();

            for (int i = 0; i < distancias.Count; i++)
                distancias[i] = distancias[i] / suma;

            return distancias;
        }

        public override string Clasificar(Muestra muestraComprobar)
        {
            var distancias = CalcularPertenenciaClases(muestraComprobar);

            var resultado = "";

            for (var i = 0; i < Datos.Count; i++)
            {
                resultado += $"{Datos[i].Nombre}: ";
                resultado += distancias[i].ToString("F");
                if (i != Datos.Count - 1) resultado += "\n";
            }

            return resultado;
        }

        public override void Entrenar()
        {
            CentrosIniciales();

            var numeroMuestras = Muestras.Count;
            var numeroClases = this.Datos.Count();
            var dimensionMuestra = Muestras.First().Medidas.Count();

            do
            {
                //Crear matriz pertenencia
                double[][] pertenencia = new double[numeroClases][];
                for (int i = 0; i < numeroClases; i++)
                    pertenencia[i] = new double[numeroMuestras];

                //Calculamos matriz pertenencia
                for (int j = 0; j < numeroMuestras; j++)
                {
                    List<double> distancias = CalcularPertenenciaClases(Muestras[j]);

                    for (int i = 0; i < numeroClases; i++)
                        pertenencia[i][j] = distancias[i];
                }

                //Calculamos los nuevos centros para cada clase
                for (int i = 0; i < numeroClases; i++)
                {
                    //Calculamos la suma para el divisor
                    double sumaClase = pertenencia[i].Sum(f => Math.Pow(f, PesoExponencialB));
                    //double sumaClase = pertenencia[i].Sum();

                    //Calculamos la suma para el dividendo
                    double[][] sumaParcialDividendo = new double[dimensionMuestra][];
                    for (int l = 0; l < dimensionMuestra; l++)
                        sumaParcialDividendo[l] = new double[numeroMuestras];

                    for (int j = 0; j < numeroMuestras; j++)
                    for (int k = 0; k < dimensionMuestra; k++)
                        sumaParcialDividendo[k][j] +=
                            Muestras[j].Medidas[k] * Math.Pow(pertenencia[i][j], PesoExponencialB);

                    //Actualizamos los valores del centro de cada clase
                    for (int m = 0; m < dimensionMuestra; m++)
                        Datos[i].Centro.Medidas[m] = sumaParcialDividendo[m].Sum(f => f) / sumaClase;
                }
            } while (SeguirCalculando());

            //Una vez que hemos acabado el entrenamiento actualizamos los centros
            for (int i = 0; i < Centros.Count; i++)
            for (int j = 0; j < Centros[i].Medidas.Count; j++)
                Centros[i].Medidas[j] = Datos[i].Centro.Medidas[j];
        }

        private bool SeguirCalculando()
        {
            //Condicion de seguridad, si llevamos mas de 10000 iteraciones paramos de entrenar
            if (++iteraciones >= 10000)
                return false;

            //Comparamos la diferencia entre los centros iniciales y finales
            //si es menor que el epsilon marcado paramos en caso contrario
            //seguimos entrenando
            bool resultado = false;

            for (int i = 0; i < Centros.Count; i++)
                if (this.Distancia(Centros[i], Datos[i].Centro) > Tolerancia)
                    resultado = true;

            return resultado;
        }
    }
}