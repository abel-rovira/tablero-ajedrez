using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace TableroAjedrez
{
    public partial class VentanaPrincipal : Window
    {
        private readonly Button[,] casillas = new Button[8, 8];
        private readonly PartidaAjedrez partida = new PartidaAjedrez();
        private readonly List<Movimiento> movimientosLegalesSeleccionados = new List<Movimiento>();

        private Posicion? casillaSeleccionada;

        private readonly Brush casillaClara = new SolidColorBrush(Color.FromRgb(250, 232, 200));
        private readonly Brush casillaOscura = new SolidColorBrush(Color.FromRgb(162, 59, 59));
        private readonly Brush pincelSeleccion = new SolidColorBrush(Color.FromRgb(223, 172, 83));
        private readonly Brush pincelLegal = new SolidColorBrush(Color.FromRgb(181, 128, 68));
        private readonly Brush pincelCaptura = new SolidColorBrush(Color.FromRgb(126, 38, 38));
        private readonly Brush pincelJaque = new SolidColorBrush(Color.FromRgb(198, 61, 47));

        public VentanaPrincipal()
        {
            InitializeComponent();
            ConstruirTablero();
            Dibujar();
        }

        private void ConstruirTablero()
        {
            CuadriculaTablero.Children.Clear();

            for (int fila = 0; fila < 8; fila++)
            {
                for (int columna = 0; columna < 8; columna++)
                {
                    var boton = new Button
                    {
                        BorderBrush = Brushes.Transparent,
                        BorderThickness = new Thickness(0),
                        Padding = new Thickness(0),
                        Tag = new Posicion(fila, columna),
                        Cursor = Cursors.Hand,
                        FocusVisualStyle = null
                    };

                    boton.Click += Casilla_Click;
                    casillas[fila, columna] = boton;
                    CuadriculaTablero.Children.Add(boton);
                }
            }
        }

        private void Casilla_Click(object emisor, RoutedEventArgs argumentos)
        {
            if (partida.EstaTerminada)
            {
                return;
            }

            var casillaPulsada = (Posicion)((Button)emisor).Tag;
            var movimientoSeleccionado = movimientosLegalesSeleccionados.FirstOrDefault(m => MismaPosicion(m.Hasta, casillaPulsada));

            if (movimientoSeleccionado != null)
            {
                partida.HacerMovimiento(movimientoSeleccionado);
                casillaSeleccionada = null;
                movimientosLegalesSeleccionados.Clear();
                Dibujar();
                return;
            }

            var pieza = partida.ObtenerPieza(casillaPulsada);
            if (pieza != null && pieza.Color == partida.Turno)
            {
                casillaSeleccionada = casillaPulsada;
                movimientosLegalesSeleccionados.Clear();
                movimientosLegalesSeleccionados.AddRange(partida.ObtenerMovimientosLegales(casillaPulsada));
            }
            else
            {
                casillaSeleccionada = null;
                movimientosLegalesSeleccionados.Clear();
            }

            Dibujar();
        }

        private void Dibujar()
        {
            for (int filaMostrada = 0; filaMostrada < 8; filaMostrada++)
            {
                for (int columnaMostrada = 0; columnaMostrada < 8; columnaMostrada++)
                {
                    int fila = filaMostrada;
                    int columna = columnaMostrada;
                    var boton = casillas[filaMostrada, columnaMostrada];
                    var posicion = new Posicion(fila, columna);
                    var pieza = partida.ObtenerPieza(posicion);

                    boton.Tag = posicion;
                    boton.Content = pieza == null ? null : CrearVistaPieza(pieza);
                    boton.Background = ObtenerPincelCasilla(posicion);
                }
            }

            TextoEstado.Text = CrearTextoEstado();
            ActualizarPanelFinPartida();
        }

        private Brush ObtenerPincelCasilla(Posicion posicion)
        {
            if (casillaSeleccionada.HasValue && MismaPosicion(casillaSeleccionada.Value, posicion))
            {
                return pincelSeleccion;
            }

            if (partida.EstaEnJaque(partida.Turno))
            {
                var rey = partida.BuscarRey(partida.Turno);
                if (rey.HasValue && MismaPosicion(rey.Value, posicion))
                {
                    return pincelJaque;
                }
            }

            var movimientoLegal = movimientosLegalesSeleccionados.FirstOrDefault(m => MismaPosicion(m.Hasta, posicion));
            if (movimientoLegal != null)
            {
                return partida.ObtenerPieza(posicion) != null || movimientoLegal.EsAlPaso ? pincelCaptura : pincelLegal;
            }

            return (posicion.Fila + posicion.Columna) % 2 == 0 ? casillaClara : casillaOscura;
        }

        private string CrearTextoEstado()
        {
            if (partida.Resultado == ResultadoPartida.JaqueMate)
            {
                return "Jaque mate";
            }

            if (partida.Resultado == ResultadoPartida.Ahogado)
            {
                return "Tablas por ahogado";
            }

            if (partida.EstaEnJaque(partida.Turno))
            {
                return "Jaque a las " + NombreColor(partida.Turno).ToLowerInvariant();
            }

            return "Turno: " + NombreColor(partida.Turno);
        }

        private void ActualizarPanelFinPartida()
        {
            if (!partida.EstaTerminada)
            {
                PanelFinPartida.Visibility = Visibility.Collapsed;
                return;
            }

            TextoFinPartida.Text = partida.Resultado == ResultadoPartida.JaqueMate
                ? "Jaque mate. Ganan las " + NombreColor(PartidaAjedrez.Contrario(partida.Turno)).ToLowerInvariant() + "."
                : "Tablas por ahogado.";
            PanelFinPartida.Visibility = Visibility.Visible;
        }

        private UIElement CrearVistaPieza(Pieza pieza)
        {
            var lienzo = new Canvas
            {
                Width = 92,
                Height = 92
            };

            if (pieza.Tipo == TipoPieza.Torre)
            {
                var grupoTorre = new StackPanel { Margin = new Thickness(18, 9, 0, 0) };
                grupoTorre.Children.Add(CrearTrazoPieza("RookHead"));
                grupoTorre.Children.Add(CrearTrazoPieza("RookBody"));
                lienzo.Children.Add(grupoTorre);
            }
            else if (pieza.Tipo == TipoPieza.Alfil)
            {
                lienzo.Children.Add(CrearTrazoPieza("BishopHelmetFront"));
                lienzo.Children.Add(CrearTrazoPieza("BishopHelmet"));
            }
            else if (pieza.Tipo == TipoPieza.Caballo)
            {
                lienzo.Children.Add(CrearTrazoPieza("Caballo"));
            }
            else if (pieza.Tipo == TipoPieza.Rey)
            {
                lienzo.Children.Add(CrearTrazoPieza("Rey"));
            }
            else if (pieza.Tipo == TipoPieza.Dama)
            {
                lienzo.Children.Add(CrearTrazoPieza("Dama"));
            }
            else
            {
                lienzo.Children.Add(CrearTrazoPieza("Peon"));
            }

            if (pieza.Color == ColorPieza.Negras)
            {
                lienzo.RenderTransformOrigin = new Point(0.5, 0.5);
                lienzo.RenderTransform = new RotateTransform(180);
            }

            return new Viewbox
            {
                Stretch = Stretch.Uniform,
                Child = lienzo,
                Margin = new Thickness(4)
            };
        }

        private Path CrearTrazoPieza(string nombreEstilo)
        {
            return new Path
            {
                Style = (Style)FindResource(nombreEstilo),
                Fill = new SolidColorBrush(Color.FromRgb(45, 23, 26))
            };
        }

        private string NombreColor(ColorPieza color)
        {
            return color == ColorPieza.Blancas ? "Blancas" : "Negras";
        }

        private bool MismaPosicion(Posicion una, Posicion otra)
        {
            return una.Fila == otra.Fila && una.Columna == otra.Columna;
        }

        private void BotonNuevaPartida_Click(object emisor, RoutedEventArgs argumentos)
        {
            partida.IniciarNuevaPartida();
            casillaSeleccionada = null;
            movimientosLegalesSeleccionados.Clear();
            Dibujar();
        }

        private void BotonSalir_Click(object emisor, RoutedEventArgs argumentos)
        {
            Close();
        }
    }
}
