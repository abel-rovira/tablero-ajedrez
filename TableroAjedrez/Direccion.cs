namespace TableroAjedrez
{
    public struct Direccion
    {
        public Direccion(int deltaFila, int deltaColumna)
        {
            DeltaFila = deltaFila;
            DeltaColumna = deltaColumna;
        }

        public int DeltaFila { get; }
        public int DeltaColumna { get; }
    }
}
