using System.Collections.Generic;
using AlgoritmoAEstrella.Data;

namespace AlgoritmoAEstrella.Algoritmia
{
    public static class Algoritmia
    {
        /// <summary>
        /// Calcula con el algoritmo A* el camino entre el inicio y la meta en el mapa proporcionado
        /// </summary>
        /// <param name="inicio">Coordenada de inicio</param>
        /// <param name="meta">Coordenada de llegada</param>
        /// <param name="movimientoDiagonal">Indica si se permite el movimiento lateral</param>
        /// <param name="movimientoOrtogonal"></param>
        /// <param name="mapa">Mapa en el que se ejecuta el algoritmo</param>
        /// <returns>Objeto Resultado con el camino y el coste.</returns>
        public static AEstrellaResultado CalculoAEstrella(Baldosa inicio, Baldosa meta, bool movimientoDiagonal,
            bool movimientoOrtogonal, Baldosa[,] mapa)
        {
            var aEstrella =
                new AEstrella(inicio, meta, movimientoDiagonal, movimientoOrtogonal, mapa);
            return aEstrella.Algoritmo();
        }

        /// <summary>
        /// Calcula con el algoritmo A* el camino entre el inicio y la meta en el mapa proporcionado
        /// </summary>
        /// <param name="inicio">Coordenada de inicio</param>
        /// <param name="meta">Coordenada de llegada</param>
        /// <param name="movimientoDiagonal">Indica si se permite el movimiento lateral</param>
        /// <param name="movimientoOrtogonal"></param>
        /// <param name="mapa">Mapa en el que se ejecuta el algoritmo</param>
        /// <param name="wayPoints">Puntos intermedios por los que hay que pasar en orden</param>
        /// <returns>Objeto Resultado con el camino y el coste.</returns>
        public static AEstrellaResultado CalculoAEstrella(Baldosa inicio, Baldosa meta, bool movimientoDiagonal,
            bool movimientoOrtogonal, Baldosa[,] mapa, params Baldosa[] wayPoints)
        {
            var resultado = new AEstrellaResultado(new List<Baldosa>(), 0);
            var puntos = new Baldosa[wayPoints.Length + 2];

            var i = 0;
            puntos[i++] = inicio;
            for (; i <= wayPoints.Length; i++)
            {
                puntos[i] = wayPoints[i - 1];
            }

            puntos[i] = meta;

            i = 1;
            for (; i < puntos.Length; i++)
            {
                var aEstrella = new AEstrella(puntos[i - 1], puntos[i], movimientoDiagonal, movimientoOrtogonal,
                    mapa);
                var parcial = aEstrella.Algoritmo();
                if (parcial.Camino == null)
                {
                    return new AEstrellaResultado(mapa, null, 0.0);
                }

                resultado.Coste += parcial.Coste;
                resultado.Camino.AddRange(parcial.Camino);
                aEstrella.LimpiarListas(resultado.Camino);
            }

            return resultado;
        }
    }
}