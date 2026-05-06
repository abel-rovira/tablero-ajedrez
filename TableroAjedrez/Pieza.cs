namespace TableroAjedrez
{
    public sealed class Pieza
    {
        public Pieza(TipoPieza tipo, ColorPieza color)
        {
            Tipo = tipo;
            Color = color;
        }

        public TipoPieza Tipo { get; set; }
        public ColorPieza Color { get; }
        public bool SeHaMovido { get; set; }
    }
}
