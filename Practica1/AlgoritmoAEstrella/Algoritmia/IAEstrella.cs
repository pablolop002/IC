using System.Collections.Generic;
using AlgoritmoAEstrella.Data;
using Priority_Queue;

namespace AlgoritmoAEstrella.Algoritmia
{
    internal abstract class IaEstrella
    {
        /// <summary>
        /// Llamada al algoritmo
        /// </summary>
        /// <returns>Objeto con el camino y el coste asociado</returns>
        public abstract AEstrellaResultado Algoritmo();

        /// <summary>
        /// Origen del algoritmo
        /// </summary>
        protected Baldosa Inicio { get; set; }

        /// <summary>
        /// Meta del algoritmo
        /// </summary>
        protected Baldosa Meta { get; set; }

        /// <summary>
        /// Mapa del algoritmo
        /// </summary>
        protected Baldosa[,] Mapa { get; set; }

        /// <summary>
        /// Indica si se puede nadar
        /// </summary>
        public bool Nadar { get; set; }

        /// <summary>
        /// Indica si se puede escalar
        /// </summary>
        public bool Escalar { get; set; }

        /// <summary>
        /// Lista abierta para el algoritmo 
        /// </summary>
        protected FastPriorityQueue<Baldosa> Abierta { get; set; }

        /// <summary>
        /// Puntos intermedios por los que pasar
        /// </summary>
        ///protected Coordenada[] Waypoints { get; set; }

        /// <summary>
        /// Indica si se permite la expansión de nodos diagonales en el mapa
        /// </summary>
        protected bool MovimientoDiagonal { get; set; }

        /// <summary>
        /// Indica si se permite la expansión de nodos ortogonales en el mapa
        /// </summary>
        protected bool MovimientoOrtogonal { get; set; }

        /// <summary>
        /// Trata y genera la solución del algoritmo
        /// </summary>
        /// <param name="resultadoAlgoritmo">Punto del mapa al que se llega</param>
        /// <returns>Objeto con el camino y el coste asociado</returns>
        protected AEstrellaResultado ProcesarResultado(Baldosa resultadoAlgoritmo)
        {
            AEstrellaResultado resultado;
            if (resultadoAlgoritmo == null)
            {
                resultado = new AEstrellaResultado(Mapa, null, 0.0);
            }
            else
            {
                resultado = new AEstrellaResultado(Mapa, new List<Baldosa>(), resultadoAlgoritmo.F);
                TratarCaminoResultado(resultadoAlgoritmo, resultado);
            }

            return resultado;
        }

        /// <summary>
        /// Genera el camino resultado
        /// </summary>
        /// <param name="resultadoAlgoritmo">Punto donde acabael algoritmo</param>
        /// <param name="resultado">Objeto resultado</param>
        protected static void TratarCaminoResultado(Baldosa resultadoAlgoritmo, AEstrellaResultado resultado)
        {
            Baldosa actual = resultadoAlgoritmo;
            while (actual != null)
            {
                resultado.Camino.Add(actual);
                actual = actual.Padre;
            }

            resultado.Camino.Reverse();
        }

        /// <summary>
        /// Operaciones de limpieza del algoritmo
        /// </summary>
        protected internal void LimpiarListas(List<Baldosa> camino)
        {
            Abierta.Clear();
            foreach (var item in Mapa)
            {
                Abierta.ResetNode(item);
                if (!camino.Contains(item))
                    //if (item.Abierto == true)
                {
                    item.Abierto = null;
                }
            }
        }
    }
}