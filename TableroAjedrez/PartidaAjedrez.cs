using System;
using System.Collections.Generic;
using System.Linq;

namespace TableroAjedrez
{
    public sealed class PartidaAjedrez
    {
        private readonly Pieza[,] tablero = new Pieza[8, 8];
        private Posicion? objetivoAlPaso;

        public PartidaAjedrez()
        {
            IniciarNuevaPartida();
        }

        public Pieza[,] Tablero => tablero;
        public ColorPieza Turno { get; private set; }
        public ResultadoPartida Resultado { get; private set; }

        public bool EstaTerminada => Resultado != ResultadoPartida.EnCurso;

        public void IniciarNuevaPartida()
        {
            Array.Clear(tablero, 0, tablero.Length);
            objetivoAlPaso = null;
            Turno = ColorPieza.Blancas;
            Resultado = ResultadoPartida.EnCurso;

            ColocarFilaPrincipal(0, ColorPieza.Negras);
            ColocarPeones(1, ColorPieza.Negras);
            ColocarPeones(6, ColorPieza.Blancas);
            ColocarFilaPrincipal(7, ColorPieza.Blancas);
        }

        public Pieza ObtenerPieza(Posicion posicion)
        {
            return tablero[posicion.Fila, posicion.Columna];
        }

        public List<Movimiento> ObtenerMovimientosLegales(Posicion desde)
        {
            var pieza = tablero[desde.Fila, desde.Columna];
            if (pieza == null)
            {
                return new List<Movimiento>();
            }

            return ObtenerMovimientosPseudoLegales(desde)
                .Where(movimiento => !DejariaReyEnJaque(movimiento, pieza.Color))
                .ToList();
        }

        public void HacerMovimiento(Movimiento movimiento)
        {
            var pieza = tablero[movimiento.Desde.Fila, movimiento.Desde.Columna];
            if (pieza == null || EstaTerminada)
            {
                return;
            }

            if (movimiento.EsAlPaso)
            {
                int filaPeonCapturado = pieza.Color == ColorPieza.Blancas ? movimiento.Hasta.Fila + 1 : movimiento.Hasta.Fila - 1;
                tablero[filaPeonCapturado, movimiento.Hasta.Columna] = null;
            }

            tablero[movimiento.Hasta.Fila, movimiento.Hasta.Columna] = pieza;
            tablero[movimiento.Desde.Fila, movimiento.Desde.Columna] = null;

            if (movimiento.EsEnroque)
            {
                int columnaTorreDesde = movimiento.Hasta.Columna == 6 ? 7 : 0;
                int columnaTorreHasta = movimiento.Hasta.Columna == 6 ? 5 : 3;
                var torre = tablero[movimiento.Desde.Fila, columnaTorreDesde];
                tablero[movimiento.Desde.Fila, columnaTorreHasta] = torre;
                tablero[movimiento.Desde.Fila, columnaTorreDesde] = null;
                if (torre != null)
                {
                    torre.SeHaMovido = true;
                }
            }

            if (movimiento.Promocion.HasValue)
            {
                pieza.Tipo = movimiento.Promocion.Value;
            }

            pieza.SeHaMovido = true;
            objetivoAlPaso = null;
            if (pieza.Tipo == TipoPieza.Peon && Math.Abs(movimiento.Hasta.Fila - movimiento.Desde.Fila) == 2)
            {
                objetivoAlPaso = new Posicion((movimiento.Desde.Fila + movimiento.Hasta.Fila) / 2, movimiento.Desde.Columna);
            }

            Turno = Contrario(Turno);
            ActualizarResultado();
        }

        public bool EstaEnJaque(ColorPieza color)
        {
            var rey = BuscarRey(color);
            return rey.HasValue && CasillaEstaAtacada(rey.Value, Contrario(color));
        }

        public bool TieneAlgunMovimientoLegal(ColorPieza color)
        {
            for (int fila = 0; fila < 8; fila++)
            {
                for (int columna = 0; columna < 8; columna++)
                {
                    var pieza = tablero[fila, columna];
                    if (pieza != null && pieza.Color == color && ObtenerMovimientosLegales(new Posicion(fila, columna)).Any())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public Posicion? BuscarRey(ColorPieza color)
        {
            for (int fila = 0; fila < 8; fila++)
            {
                for (int columna = 0; columna < 8; columna++)
                {
                    var pieza = tablero[fila, columna];
                    if (pieza != null && pieza.Color == color && pieza.Tipo == TipoPieza.Rey)
                    {
                        return new Posicion(fila, columna);
                    }
                }
            }

            return null;
        }

        public static ColorPieza Contrario(ColorPieza color)
        {
            return color == ColorPieza.Blancas ? ColorPieza.Negras : ColorPieza.Blancas;
        }

        private void ColocarFilaPrincipal(int fila, ColorPieza color)
        {
            tablero[fila, 0] = new Pieza(TipoPieza.Torre, color);
            tablero[fila, 1] = new Pieza(TipoPieza.Caballo, color);
            tablero[fila, 2] = new Pieza(TipoPieza.Alfil, color);
            tablero[fila, 3] = new Pieza(TipoPieza.Dama, color);
            tablero[fila, 4] = new Pieza(TipoPieza.Rey, color);
            tablero[fila, 5] = new Pieza(TipoPieza.Alfil, color);
            tablero[fila, 6] = new Pieza(TipoPieza.Caballo, color);
            tablero[fila, 7] = new Pieza(TipoPieza.Torre, color);
        }

        private void ColocarPeones(int fila, ColorPieza color)
        {
            for (int columna = 0; columna < 8; columna++)
            {
                tablero[fila, columna] = new Pieza(TipoPieza.Peon, color);
            }
        }

        private void ActualizarResultado()
        {
            if (TieneAlgunMovimientoLegal(Turno))
            {
                Resultado = ResultadoPartida.EnCurso;
                return;
            }

            Resultado = EstaEnJaque(Turno) ? ResultadoPartida.JaqueMate : ResultadoPartida.Ahogado;
        }

        private List<Movimiento> ObtenerMovimientosPseudoLegales(Posicion desde)
        {
            var pieza = tablero[desde.Fila, desde.Columna];
            var movimientos = new List<Movimiento>();
            if (pieza == null)
            {
                return movimientos;
            }

            if (pieza.Tipo == TipoPieza.Peon)
            {
                AgregarMovimientosPeon(desde, pieza, movimientos);
            }
            else if (pieza.Tipo == TipoPieza.Caballo)
            {
                AgregarMovimientosCaballo(desde, pieza, movimientos);
            }
            else if (pieza.Tipo == TipoPieza.Alfil)
            {
                AgregarMovimientosDeslizantes(desde, pieza, movimientos, DireccionesDiagonales());
            }
            else if (pieza.Tipo == TipoPieza.Torre)
            {
                AgregarMovimientosDeslizantes(desde, pieza, movimientos, DireccionesRectas());
            }
            else if (pieza.Tipo == TipoPieza.Dama)
            {
                AgregarMovimientosDeslizantes(desde, pieza, movimientos, DireccionesRectas().Concat(DireccionesDiagonales()).ToArray());
            }
            else if (pieza.Tipo == TipoPieza.Rey)
            {
                AgregarMovimientosRey(desde, pieza, movimientos);
            }

            return movimientos;
        }

        private void AgregarMovimientosPeon(Posicion desde, Pieza pieza, List<Movimiento> movimientos)
        {
            int direccionPeon = pieza.Color == ColorPieza.Blancas ? -1 : 1;
            int filaInicial = pieza.Color == ColorPieza.Blancas ? 6 : 1;
            int filaPromocion = pieza.Color == ColorPieza.Blancas ? 0 : 7;
            int filaSiguiente = desde.Fila + direccionPeon;

            if (EstaDentro(filaSiguiente, desde.Columna) && tablero[filaSiguiente, desde.Columna] == null)
            {
                AgregarMovimientoPeon(desde, new Posicion(filaSiguiente, desde.Columna), movimientos, filaPromocion);

                int filaDoble = desde.Fila + direccionPeon * 2;
                if (desde.Fila == filaInicial && tablero[filaDoble, desde.Columna] == null)
                {
                    movimientos.Add(new Movimiento(desde, new Posicion(filaDoble, desde.Columna)));
                }
            }

            for (int dc = -1; dc <= 1; dc += 2)
            {
                int columna = desde.Columna + dc;
                if (!EstaDentro(filaSiguiente, columna))
                {
                    continue;
                }

                var destino = tablero[filaSiguiente, columna];
                if (destino != null && destino.Color != pieza.Color && destino.Tipo != TipoPieza.Rey)
                {
                    AgregarMovimientoPeon(desde, new Posicion(filaSiguiente, columna), movimientos, filaPromocion);
                }
                else if (objetivoAlPaso.HasValue && objetivoAlPaso.Value.Fila == filaSiguiente && objetivoAlPaso.Value.Columna == columna)
                {
                    movimientos.Add(new Movimiento(desde, new Posicion(filaSiguiente, columna)) { EsAlPaso = true });
                }
            }
        }

        private void AgregarMovimientoPeon(Posicion desde, Posicion hasta, List<Movimiento> movimientos, int filaPromocion)
        {
            var movimiento = new Movimiento(desde, hasta);
            if (hasta.Fila == filaPromocion)
            {
                movimiento.Promocion = TipoPieza.Dama;
            }

            movimientos.Add(movimiento);
        }

        private void AgregarMovimientosCaballo(Posicion desde, Pieza pieza, List<Movimiento> movimientos)
        {
            int[,] desplazamientos = { { -2, -1 }, { -2, 1 }, { -1, -2 }, { -1, 2 }, { 1, -2 }, { 1, 2 }, { 2, -1 }, { 2, 1 } };
            for (int i = 0; i < desplazamientos.GetLength(0); i++)
            {
                AgregarMovimientoUnPaso(desde, pieza, movimientos, desde.Fila + desplazamientos[i, 0], desde.Columna + desplazamientos[i, 1]);
            }
        }

        private void AgregarMovimientosRey(Posicion desde, Pieza pieza, List<Movimiento> movimientos)
        {
            for (int df = -1; df <= 1; df++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (df != 0 || dc != 0)
                    {
                        AgregarMovimientoUnPaso(desde, pieza, movimientos, desde.Fila + df, desde.Columna + dc);
                    }
                }
            }

            AgregarEnroques(desde, pieza, movimientos);
        }

        private void AgregarMovimientoUnPaso(Posicion desde, Pieza pieza, List<Movimiento> movimientos, int fila, int columna)
        {
            if (!EstaDentro(fila, columna))
            {
                return;
            }

            var destino = tablero[fila, columna];
            if (destino == null || (destino.Color != pieza.Color && destino.Tipo != TipoPieza.Rey))
            {
                movimientos.Add(new Movimiento(desde, new Posicion(fila, columna)));
            }
        }

        private void AgregarMovimientosDeslizantes(Posicion desde, Pieza pieza, List<Movimiento> movimientos, Direccion[] direcciones)
        {
            foreach (var direccion in direcciones)
            {
                int fila = desde.Fila + direccion.DeltaFila;
                int columna = desde.Columna + direccion.DeltaColumna;

                while (EstaDentro(fila, columna))
                {
                    var destino = tablero[fila, columna];
                    if (destino == null)
                    {
                        movimientos.Add(new Movimiento(desde, new Posicion(fila, columna)));
                    }
                    else
                    {
                        if (destino.Color != pieza.Color && destino.Tipo != TipoPieza.Rey)
                        {
                            movimientos.Add(new Movimiento(desde, new Posicion(fila, columna)));
                        }

                        break;
                    }

                    fila += direccion.DeltaFila;
                    columna += direccion.DeltaColumna;
                }
            }
        }

        private void AgregarEnroques(Posicion desde, Pieza rey, List<Movimiento> movimientos)
        {
            if (rey.SeHaMovido || EstaEnJaque(rey.Color))
            {
                return;
            }

            IntentarAgregarEnroque(desde, rey, movimientos, 7, 6, 5);
            IntentarAgregarEnroque(desde, rey, movimientos, 0, 2, 3);
        }

        private void IntentarAgregarEnroque(Posicion desde, Pieza rey, List<Movimiento> movimientos, int columnaTorre, int columnaDestinoRey, int columnaPaso)
        {
            var torre = tablero[desde.Fila, columnaTorre];
            if (torre == null || torre.Tipo != TipoPieza.Torre || torre.Color != rey.Color || torre.SeHaMovido)
            {
                return;
            }

            int paso = columnaTorre > desde.Columna ? 1 : -1;
            for (int columna = desde.Columna + paso; columna != columnaTorre; columna += paso)
            {
                if (tablero[desde.Fila, columna] != null)
                {
                    return;
                }
            }

            if (CasillaEstaAtacada(new Posicion(desde.Fila, columnaPaso), Contrario(rey.Color)) ||
                CasillaEstaAtacada(new Posicion(desde.Fila, columnaDestinoRey), Contrario(rey.Color)))
            {
                return;
            }

            movimientos.Add(new Movimiento(desde, new Posicion(desde.Fila, columnaDestinoRey)) { EsEnroque = true });
        }

        private bool DejariaReyEnJaque(Movimiento movimiento, ColorPieza color)
        {
            var piezaMovida = tablero[movimiento.Desde.Fila, movimiento.Desde.Columna];
            var piezaCapturada = tablero[movimiento.Hasta.Fila, movimiento.Hasta.Columna];
            Pieza capturadaAlPaso = null;
            int filaCapturadaAlPaso = -1;
            int columnaTorreDesde = -1;
            int columnaTorreHasta = -1;
            Pieza torre = null;
            TipoPieza tipoOriginal = piezaMovida.Tipo;
            bool movidaOriginalmente = piezaMovida.SeHaMovido;

            if (movimiento.EsAlPaso)
            {
                filaCapturadaAlPaso = piezaMovida.Color == ColorPieza.Blancas ? movimiento.Hasta.Fila + 1 : movimiento.Hasta.Fila - 1;
                capturadaAlPaso = tablero[filaCapturadaAlPaso, movimiento.Hasta.Columna];
                tablero[filaCapturadaAlPaso, movimiento.Hasta.Columna] = null;
            }

            tablero[movimiento.Hasta.Fila, movimiento.Hasta.Columna] = piezaMovida;
            tablero[movimiento.Desde.Fila, movimiento.Desde.Columna] = null;

            if (movimiento.EsEnroque)
            {
                columnaTorreDesde = movimiento.Hasta.Columna == 6 ? 7 : 0;
                columnaTorreHasta = movimiento.Hasta.Columna == 6 ? 5 : 3;
                torre = tablero[movimiento.Desde.Fila, columnaTorreDesde];
                tablero[movimiento.Desde.Fila, columnaTorreHasta] = torre;
                tablero[movimiento.Desde.Fila, columnaTorreDesde] = null;
            }

            if (movimiento.Promocion.HasValue)
            {
                piezaMovida.Tipo = movimiento.Promocion.Value;
            }

            bool enJaque = EstaEnJaque(color);

            piezaMovida.Tipo = tipoOriginal;
            piezaMovida.SeHaMovido = movidaOriginalmente;
            tablero[movimiento.Desde.Fila, movimiento.Desde.Columna] = piezaMovida;
            tablero[movimiento.Hasta.Fila, movimiento.Hasta.Columna] = piezaCapturada;

            if (movimiento.EsAlPaso)
            {
                tablero[filaCapturadaAlPaso, movimiento.Hasta.Columna] = capturadaAlPaso;
            }

            if (movimiento.EsEnroque)
            {
                tablero[movimiento.Desde.Fila, columnaTorreDesde] = torre;
                tablero[movimiento.Desde.Fila, columnaTorreHasta] = null;
            }

            return enJaque;
        }

        private bool CasillaEstaAtacada(Posicion casilla, ColorPieza porColor)
        {
            int direccionPeon = porColor == ColorPieza.Blancas ? -1 : 1;
            for (int dc = -1; dc <= 1; dc += 2)
            {
                int fila = casilla.Fila - direccionPeon;
                int columna = casilla.Columna + dc;
                if (EstaDentro(fila, columna) && TienePieza(fila, columna, TipoPieza.Peon, porColor))
                {
                    return true;
                }
            }

            int[,] desplazamientosCaballo = { { -2, -1 }, { -2, 1 }, { -1, -2 }, { -1, 2 }, { 1, -2 }, { 1, 2 }, { 2, -1 }, { 2, 1 } };
            for (int i = 0; i < desplazamientosCaballo.GetLength(0); i++)
            {
                int fila = casilla.Fila + desplazamientosCaballo[i, 0];
                int columna = casilla.Columna + desplazamientosCaballo[i, 1];
                if (EstaDentro(fila, columna) && TienePieza(fila, columna, TipoPieza.Caballo, porColor))
                {
                    return true;
                }
            }

            if (EstaAtacadaPorDeslizante(casilla, porColor, DireccionesRectas(), TipoPieza.Torre, TipoPieza.Dama))
            {
                return true;
            }

            if (EstaAtacadaPorDeslizante(casilla, porColor, DireccionesDiagonales(), TipoPieza.Alfil, TipoPieza.Dama))
            {
                return true;
            }

            for (int df = -1; df <= 1; df++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (df == 0 && dc == 0)
                    {
                        continue;
                    }

                    int fila = casilla.Fila + df;
                    int columna = casilla.Columna + dc;
                    if (EstaDentro(fila, columna) && TienePieza(fila, columna, TipoPieza.Rey, porColor))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool EstaAtacadaPorDeslizante(Posicion casilla, ColorPieza color, Direccion[] direcciones, TipoPieza tipoPieza, TipoPieza dama)
        {
            foreach (var direccion in direcciones)
            {
                int fila = casilla.Fila + direccion.DeltaFila;
                int columna = casilla.Columna + direccion.DeltaColumna;

                while (EstaDentro(fila, columna))
                {
                    var pieza = tablero[fila, columna];
                    if (pieza != null)
                    {
                        if (pieza.Color == color && (pieza.Tipo == tipoPieza || pieza.Tipo == dama))
                        {
                            return true;
                        }

                        break;
                    }

                    fila += direccion.DeltaFila;
                    columna += direccion.DeltaColumna;
                }
            }

            return false;
        }

        private bool TienePieza(int fila, int columna, TipoPieza tipo, ColorPieza color)
        {
            var pieza = tablero[fila, columna];
            return pieza != null && pieza.Tipo == tipo && pieza.Color == color;
        }

        private bool EstaDentro(int fila, int columna)
        {
            return fila >= 0 && fila < 8 && columna >= 0 && columna < 8;
        }

        private Direccion[] DireccionesRectas()
        {
            return new[]
            {
                new Direccion(-1, 0),
                new Direccion(1, 0),
                new Direccion(0, -1),
                new Direccion(0, 1)
            };
        }

        private Direccion[] DireccionesDiagonales()
        {
            return new[]
            {
                new Direccion(-1, -1),
                new Direccion(-1, 1),
                new Direccion(1, -1),
                new Direccion(1, 1)
            };
        }
    }
}
