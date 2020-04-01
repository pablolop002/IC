namespace AlgoritmoAEstrella.Data
{
    public class Coordenadas
    {
        public int X { get; }
        public int Y { get; }

        public Coordenadas(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            return obj is Coordenadas coordenada &&
                   X == coordenada.X &&
                   Y == coordenada.Y;
        }

        public override int GetHashCode()
        {
            var hashCode = 1861411795;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"X: {X.ToString()} Y: {Y.ToString()} ";
        }
    }
}