using System;
using AlgoritmoAEstrella.Data;

namespace AlgoritmoAEstrella.Algoritmia
{
    public static class Calculo
    {
        public static double Distancia(Baldosa a, Baldosa b)
        {
            return Math.Sqrt(Math.Pow(a.Posicion.X - b.Posicion.X, 2) + Math.Pow(a.Posicion.Y - b.Posicion.Y, 2));
        }

        public static double Distancia(Coordenadas a, Coordenadas b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }

        public static double Distancia(Baldosa a, Coordenadas b)
        {
            return Math.Sqrt(Math.Pow(a.Posicion.X - b.X, 2) + Math.Pow(a.Posicion.Y - b.Y, 2));
        }

        public static double Distancia(Coordenadas a, Baldosa b)
        {
            return Math.Sqrt(Math.Pow(a.X - b.Posicion.X, 2) + Math.Pow(a.Y - b.Posicion.Y, 2));
        }
    }
}