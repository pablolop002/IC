using System.Collections.Generic;
using System.Linq;

namespace MetodosClasificacion.Algoritmia
{
    public class Bayes : Aprendizaje
    {
        public int CalcularPertenenciaClases(Muestra muestraComprobar)
        {
            int resultado = -1;
            double distancia = double.MaxValue;
            for (int i = 0; i < Datos.Count; i++)
            {
                double resultadoParcial = 0;

                double[][] transpuesta = new double[1][];
                transpuesta[0] = new double[muestraComprobar.Medidas.Count];
                for (int j = 0; j < muestraComprobar.Medidas.Count; j++)
                    transpuesta[0][j] = muestraComprobar.Medidas[j];

                double[][] original = new double[muestraComprobar.Medidas.Count][];
                for (int j = 0; j < muestraComprobar.Medidas.Count; j++)
                {
                    original[j] = new double[1];
                    original[j][0] = muestraComprobar.Medidas[j];
                }

                double[][] parcial = Matrix.MatrixProduct(transpuesta, Datos[i].MatrizCovarianzaInversa);
                resultadoParcial = (Matrix.MatrixProduct(parcial, original))[0][0];

                if (resultadoParcial < distancia)
                {
                    distancia = resultadoParcial;
                    resultado = i;
                }
            }

            return resultado;
        }

        public override string Clasificar(Muestra muestraComprobar)
        {
            List<double> distancias = new List<double>();
            int i = CalcularPertenenciaClases(muestraComprobar);
            string resultado = "Pertenencias: " + Datos[i].Nombre.ToString();
            return resultado;
        }

        public override void Entrenar()
        {
            this.CentrosIniciales();
            int dimensionMedida = Datos[0].Centro.Medidas.Count;

            foreach (Clase clase in Datos)
            {
                for (int i = 0; i < dimensionMedida; i++)
                    clase.Centro.Medidas[i] =
                        Muestras.Where(f => f.NombreClase == clase.Nombre).Sum(f => f.Medidas[i])
                        /
                        Muestras.Count(f => f.NombreClase == clase.Nombre);

                clase.InicializarMatrizCovarianza(dimensionMedida);

                foreach (var item in Muestras.Where(f => f.NombreClase == clase.Nombre))
                    for (int i = 0; i < dimensionMedida; i++)
                    for (int j = 0; j < dimensionMedida; j++)
                        clase.MatrizCovarianza[i][j] +=
                            ((item.Medidas[i] - clase.Centro.Medidas[i]) * (item.Medidas[j] - clase.Centro.Medidas[j]))
                            /
                            Muestras.Count(f => f.NombreClase == clase.Nombre);
            }
        }
    }
}