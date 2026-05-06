# Tablero de Ajedrez - WPF + C#

Aplicacion de escritorio en **WPF** y **C#** para jugar una partida de ajedrez local. Mantiene el estilo visual del tablero original, con piezas dibujadas mediante recursos XAML, y anade logica de juego para que el tablero sea funcional.

## Caracteristicas

- Tablero 8x8 con estilo visual clasico en rojo y crema.
- Piezas dibujadas con `Path` y recursos XAML.
- Movimiento de piezas por clic.
- Validacion de movimientos legales.
- Turnos de blancas y negras.
- Capturas normales.
- Jaque, jaque mate y tablas por ahogado.
- Enroque corto y largo.
- Captura al paso.
- Promocion automatica a dama.
- Pantalla central al terminar la partida con:
  - `Nueva partida`
  - `Salir`

## Tecnologias utilizadas

- **C#**
- **WPF (Windows Presentation Foundation)**
- **XAML**
- **.NET 10 Windows**

## Estructura del proyecto

```text
TableroAjedrez/
|-- App.xaml
|-- App.xaml.cs
|-- VentanaPrincipal.xaml
|-- VentanaPrincipal.xaml.cs
|-- PartidaAjedrez.cs
|-- Pieza.cs
|-- Movimiento.cs
|-- Posicion.cs
|-- Direccion.cs
|-- ColorPieza.cs
|-- TipoPieza.cs
|-- ResultadoPartida.cs
`-- TableroAjedrez.csproj
```

En la raiz del repositorio:

```text
TableroAjedrez.sln
README.md
AGENTS.md
.gitignore
NuGet.Config
```

## Como ejecutar el proyecto

Desde PowerShell:

```powershell
cd C:\Users\Usuario\Documents\tablero-ajedrez
dotnet run --project .\TableroAjedrez\TableroAjedrez.csproj
```

Tambien puedes abrir la solucion en Visual Studio:

```text
C:\Users\Usuario\Documents\tablero-ajedrez\TableroAjedrez.sln
```

Y pulsar **F5** o **Iniciar**.

## Como jugar

1. Haz clic en una pieza del color que tiene el turno.
2. Las casillas disponibles se resaltan.
3. Haz clic en una casilla resaltada para mover.
4. Si la partida termina, apareceran las opciones `Nueva partida` y `Salir` en el centro.

## Compilar

```powershell
cd C:\Users\Usuario\Documents\tablero-ajedrez
dotnet build .\TableroAjedrez.sln --configfile .\NuGet.Config
```

## Vista previa

<div align="center">
  <img width="875" height="872" alt="image" src="https://github.com/user-attachments/assets/878e968e-ce05-4224-a798-1fb56ef30c7c" />
</div>
