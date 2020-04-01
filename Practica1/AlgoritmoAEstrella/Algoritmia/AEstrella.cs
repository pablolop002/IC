using AlgoritmoAEstrella.Data;
using Priority_Queue;
using Xamarin.Essentials;
using Xamarin.Forms;

// ReSharper disable CompareOfFloatsByEqualityOperator

namespace AlgoritmoAEstrella.Algoritmia
{
    class AEstrella : IaEstrella
    {
        internal AEstrella(Baldosa inicio, Baldosa meta, bool movimientoDiagonal, bool movimientoOrtogonal,
            Baldosa[,] mapa)
        {
            Inicio = inicio;
            Meta = meta;
            Mapa = mapa;
            Abierta = new FastPriorityQueue<Baldosa>(mapa.Length);
            Nadar = true;
            Escalar = true;
            MovimientoDiagonal = movimientoDiagonal;
            MovimientoOrtogonal = movimientoOrtogonal;
        }

        /// <summary>
        /// Llamada al algoritmo
        /// </summary>
        /// <returns>Objeto con el camino y el coste asociado</returns>
        public override AEstrellaResultado Algoritmo()
        {
            AEstrellaResultado resultado;

            // Calculamos la heurística entre el inicio y el final
            Mapa[Inicio.Posicion.X, Inicio.Posicion.Y].H = Calculo.Distancia(Inicio, Meta);

            // Agregamos el nodo inicio a la lista abierta
            Mapa[Inicio.Posicion.X, Inicio.Posicion.Y].Abierto = true;
            Abierta.Enqueue(Mapa[Inicio.Posicion.X, Inicio.Posicion.Y], Mapa[Inicio.Posicion.X, Inicio.Posicion.Y].F);

            // Ejecutamos el algoritmo
            var punto = CalculoAlgoritmo();

            // Procesamos el resultado
            resultado = ProcesarResultado(punto);
            return resultado;
        }

        /// <summary>
        /// Ejecución del algoritmo
        /// </summary>
        /// <returns>Punto del mapa al que se llega</returns>
        private Baldosa CalculoAlgoritmo()
        {
            Baldosa resultado = null;
            var salir = false;

            while (!salir)
            {
                if (Abierta.Count == 0)
                    salir = true;
                else
                {
                    var actual = Abierta.Dequeue();

                    //Cerramos el nodo
                    actual.Abierto = false;
                    MainThread.BeginInvokeOnMainThread(() => { actual.Celda.BackgroundColor = Color.Teal; });

                    if (actual.Posicion.Equals(Meta.Posicion))
                    {
                        resultado = Mapa[Meta.Posicion.X, Meta.Posicion.Y];
                        salir = true;
                    }
                    else
                    {
                        // Expansion del punto actual
                        if (MovimientoDiagonal && MovimientoOrtogonal)
                            ExpansionConDiagonales(actual);
                        else
                        {
                            if (MovimientoOrtogonal)
                                ExpansionOrtogonal(actual);
                            if (MovimientoDiagonal)
                                ExpansionDiagonal(actual);
                        }
                    }
                }
            }

            return resultado;
        }

        private void ExpansionConDiagonales(Baldosa actual)
        {
            for (var i = -1; i <= 1; i++)
            {
                if (actual.Posicion.X + i >= 0 && actual.Posicion.X + i < Mapa.GetLength(0))
                {
                    for (var j = -1; j <= 1; j++)
                    {
                        if (actual.Posicion.Y + j >= 0 && actual.Posicion.Y + j < Mapa.GetLength(1))
                        {
                            if (Mapa[actual.Posicion.X + i, actual.Posicion.Y + j].Accesible)
                            {
                                if (!(Mapa[actual.Posicion.X + i, actual.Posicion.Y + j].Valor ==
                                    (float) TiposBaldosa.Rio && !Nadar))
                                {
                                    if (!(Mapa[actual.Posicion.X + i, actual.Posicion.Y + j].Valor ==
                                        (float) TiposBaldosa.Montaña && !Escalar))
                                    {
                                        CostesYTratarNodos(actual, i, j);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void ExpansionOrtogonal(Baldosa actual)
        {
            for (var i = -1; i <= 1; i += 2)
            {
                if (actual.Posicion.X + i >= 0 && actual.Posicion.X + i < Mapa.GetLength(0))
                {
                    if (Mapa[actual.Posicion.X + i, actual.Posicion.Y].Accesible)
                    {
                        CostesYTratarNodos(actual, i, 0);
                    }
                }

                if (actual.Posicion.Y + i >= 0 && actual.Posicion.Y + i < Mapa.GetLength(1))
                {
                    if (Mapa[actual.Posicion.X, actual.Posicion.Y + i].Accesible)
                    {
                        CostesYTratarNodos(actual, 0, i);
                    }
                }
            }
        }

        private void ExpansionDiagonal(Baldosa actual)
        {
            for (int i = -1; i <= 1; i += 2)
            {
                if ((actual.Posicion.X + i >= 0 && actual.Posicion.X + i < Mapa.GetLength(0)) &&
                    (actual.Posicion.Y + i >= 0 && actual.Posicion.Y + i < Mapa.GetLength(1)))
                {
                    if (Mapa[actual.Posicion.X + i, actual.Posicion.Y + i].Accesible)
                    {
                        CostesYTratarNodos(actual, 0 + i, 0 + i);
                    }
                }

                if ((actual.Posicion.X + i >= 0 && actual.Posicion.X + i < Mapa.GetLength(0)) &&
                    (actual.Posicion.Y - i >= 0 && actual.Posicion.Y - i < Mapa.GetLength(1)))
                {
                    if (Mapa[actual.Posicion.X + i, actual.Posicion.Y - i].Accesible)
                    {
                        CostesYTratarNodos(actual, 0 + i, 0 - i);
                    }
                }
            }
        }

        private void CostesYTratarNodos(Baldosa actual, int i, int j)
        {
            // Calculamos los costes de acceder
            double coste = Calculo.Distancia(Mapa[actual.Posicion.X, actual.Posicion.Y],
                Mapa[actual.Posicion.X + i, actual.Posicion.Y + j]);

            if (Mapa[actual.Posicion.X + i, actual.Posicion.Y + j].Abierto == true) // Actualizamos la lista abierta
            {
                ReorientacionEnlaces(actual, i, j, coste);
            }
            else if (Mapa[actual.Posicion.X + i, actual.Posicion.Y + j].Abierto == null) // Agregamos a la lista abierta
            {
                NodoAAbierta(actual, i, j, coste);
            }
        }

        /// <summary>
        /// Pasa el nodo a la lista abierta
        /// </summary>
        /// <param name="actual">Punto donde está el algoritmo</param>
        /// <param name="i">Incremento de la coordenada X</param>
        /// <param name="j">Incremento de la coordenada Y</param>
        /// <param name="coste">Coste del paso</param>
        private void NodoAAbierta(Baldosa actual, int i, int j, double coste)
        {
            Mapa[actual.Posicion.X + i, actual.Posicion.Y + j].Abierto = true;
            Mapa[actual.Posicion.X + i, actual.Posicion.Y + j].G = Mapa[actual.Posicion.X, actual.Posicion.Y].G + coste;
            Mapa[actual.Posicion.X + i, actual.Posicion.Y + j].H =
                Calculo.Distancia(Mapa[actual.Posicion.X + i, actual.Posicion.Y + j], Meta);
            Mapa[actual.Posicion.X + i, actual.Posicion.Y + j].Padre = Mapa[actual.Posicion.X, actual.Posicion.Y];

            Abierta.Enqueue(Mapa[actual.Posicion.X + i, actual.Posicion.Y + j],
                Mapa[actual.Posicion.X + i, actual.Posicion.Y + j].F);
        }

        /// <summary>
        /// Reorienta el enlace del nodo
        /// </summary>
        /// <param name="actual">Punto donde está el algoritmo</param>
        /// <param name="i">Incremento de la coordenada X</param>
        /// <param name="j">Incremento de la coordenada Y</param>
        /// <param name="coste">Coste del paso</param>
        private void ReorientacionEnlaces(Baldosa actual, int i, int j, double coste)
        {
            if (Mapa[actual.Posicion.X + i, actual.Posicion.Y + j].G >
                Mapa[actual.Posicion.X, actual.Posicion.Y].G + coste) // Si el coste es menor
            {
                Mapa[actual.Posicion.X + i, actual.Posicion.Y + j].G =
                    Mapa[actual.Posicion.X, actual.Posicion.Y].G + coste;
                Mapa[actual.Posicion.X + i, actual.Posicion.Y + j].Padre = Mapa[actual.Posicion.X, actual.Posicion.Y];
                Abierta.UpdatePriority(Mapa[actual.Posicion.X + i, actual.Posicion.Y + j],
                    Mapa[actual.Posicion.X + i, actual.Posicion.Y + j].F);
            }
        }
    }
}