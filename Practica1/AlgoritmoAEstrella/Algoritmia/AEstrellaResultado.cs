using System.Collections.Generic;
using AlgoritmoAEstrella.Data;

namespace AlgoritmoAEstrella.Algoritmia
{
    public class AEstrellaResultado
    {
        public AEstrellaResultado(Baldosa[,] mapa)
        {
            Mapa = mapa;
        }

        public AEstrellaResultado(Baldosa[,] mapa, double coste) : this(mapa)
        {
            Coste = coste;
        }

        public AEstrellaResultado(List<Baldosa> camino, double coste)
        {
            Camino = camino;
            Coste = coste;
        }

        public AEstrellaResultado(Baldosa[,] mapa, List<Baldosa> camino, double coste) : this(mapa, coste)
        {
            Camino = camino;
        }

        public Baldosa[,] Mapa { get; internal set; }
        public List<Baldosa> Camino { get; internal set; }
        public double Coste { get; internal set; }
    }
}