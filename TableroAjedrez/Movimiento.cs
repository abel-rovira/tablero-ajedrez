namespace TableroAjedrez
{
    public sealed class Movimiento
    {
        public Movimiento(Posicion desde, Posicion hasta)
        {
            Desde = desde;
            Hasta = hasta;
        }

        public Posicion Desde { get; }
        public Posicion Hasta { get; }
        public bool EsEnroque { get; set; }
        public bool EsAlPaso { get; set; }
        public TipoPieza? Promocion { get; set; }
    }
}
