using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Priority_Queue;

namespace AlgoritmoAEstrella.Data
{
    [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
    [SuppressMessage("ReSharper", "NonReadonlyMemberInGetHashCode")]
    public class Baldosa : FastPriorityQueueNode
    {
        public Celda Celda { get; }                    // Objeto de la vista para pintarlo
        public Coordenadas Posicion { get; }           // Posición
        public Baldosa Padre { get; set; }             // Padre
        public bool? Abierto { get; set; }             // Ya analizado
        public double G { get; set; }                  // Distancia al origen
        public double H { get; set; }                  // Distancia estimada al destino
        public double Valor { get; set; }              // Coste de pasar por la baldosa
        public bool Accesible { get; set; }            // Se puede acceder a la baldosa
        public double F => G + H + Valor;              // Coste total

        public Baldosa(Coordenadas coordenadas, Celda celda, bool? abierto = null)
        {
            Posicion = coordenadas;
            Celda = celda;
            Abierto = abierto;
            G = 0;
            H = 0;
            Valor = 0;
            Accesible = true;
        }

        public override bool Equals(object obj)
        {
            return obj is Baldosa otra && Celda == otra.Celda && Posicion == otra.Posicion &&
                Padre == otra.Padre && Abierto == otra.Abierto && G == otra.G &&
                H == otra.H && Valor == otra.Valor &&
                Accesible == otra.Accesible;
        }

        public override int GetHashCode()
        {
            var hashCode = -1762213491;
            hashCode = hashCode * -1521134295 + Celda.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Coordenadas>.Default.GetHashCode(Posicion);
            hashCode = hashCode * -1521134295 + Padre.GetHashCode();
            hashCode = hashCode * -1521134295 + Abierto.GetHashCode();
            hashCode = hashCode * -1521134295 + G.GetHashCode();
            hashCode = hashCode * -1521134295 + H.GetHashCode();
            hashCode = hashCode * -1521134295 + Valor.GetHashCode();
            hashCode = hashCode * -1521134295 + Accesible.GetHashCode();
            return hashCode;
        }
    }
}