namespace MetodosClasificacion.Algoritmia
{
    public class Lloyd : Aprendizaje
    {
        private int iteraciones = 0;
        public float Tolerancia { get; set; }
        public int Iteraciones { get; set; }
        public float Aprendizaje { get; set; }

        public override string Clasificar(Muestra muestraComprobar)
        {
            string resultado = "";
            Clase clase = PertenenciaClase(muestraComprobar);

            if (clase == null)
                resultado = "No clasificable";
            else
                resultado = "Pertenencias: " + clase.Nombre.ToString();

            return resultado;
        }

        public override void Entrenar()
        {
            this.CentrosIniciales();

            do
            {
                foreach (Muestra item in Muestras)
                {
                    //Determinamos el centro mas cercano
                    Clase claseActualizar = null;
                    claseActualizar = PertenenciaClase(item);

                    //Actualizamos el centro mas cercano
                    for (int j = 0; j < claseActualizar.Centro.Medidas.Count; j++)
                        claseActualizar.Centro.Medidas[j] +=
                            Aprendizaje * (item.Medidas[j] - claseActualizar.Centro.Medidas[j]);
                }
            } while (SeguirCalculando());
        }

        private Clase PertenenciaClase(Muestra item)
        {
            Clase resultado = null;
            double distancia = double.MaxValue;
            foreach (Clase clase in Datos)
            {
                var parcial = Distancia(item, clase.Centro);
                if (parcial < distancia)
                {
                    distancia = parcial;
                    resultado = clase;
                }
            }

            return resultado;
        }

        private bool SeguirCalculando()
        {
            //Condicion de seguridad, si llevamos mas de 10000 iteraciones paramos de entrenar
            if (++iteraciones >= Iteraciones)
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