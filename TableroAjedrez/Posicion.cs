namespace TableroAjedrez
{
    public struct Posicion
    {
        public Posicion(int fila, int columna)
        {
            Fila = fila;
            Columna = columna;
        }

        public int Fila { get; }
        public int Columna { get; }
    }
}
