# AGENTS.md

Guia para agentes que trabajen en este repositorio.

## Proyecto

`TableroAjedrez` es una aplicacion de escritorio WPF en C# para Windows. El objetivo actual es ofrecer un tablero de ajedrez jugable en local, con interfaz en espanol y reglas basicas completas.

## Archivos principales

- `TableroAjedrez.sln`: solucion de Visual Studio.
- `TableroAjedrez/TableroAjedrez.csproj`: proyecto WPF principal.
- `TableroAjedrez/App.xaml`: recursos globales heredados del proyecto original.
- `TableroAjedrez/MainWindow.xaml`: interfaz principal de la aplicacion.
- `TableroAjedrez/MainWindow.xaml.cs`: estado de partida, reglas de movimiento y render del tablero.

## Reglas de trabajo

- No modificar `README.md` salvo peticion explicita del usuario.
- Mantener la interfaz visible en espanol.
- Mantener el juego local: sin red, cuentas, telemetria ni servicios externos.
- No subir artefactos generados ni datos de Visual Studio: `.vs/`, `bin/`, `obj/`, `Debug/`, `Release/`.
- Preferir cambios pequenos y faciles de revisar.
- Si se amplian reglas de ajedrez, conservar siempre una partida manual jugable con clics.
- No bloquear la UI con trabajos largos; si se anade calculo pesado, usar asincronia.

## Funcionalidad esperada

- Seleccion de piezas por clic.
- Resaltado de movimientos legales y capturas.
- Turnos de blancas y negras.
- Capturas normales.
- Jaque, jaque mate y tablas por ahogado.
- Enroque corto y largo.
- Captura al paso.
- Promocion automatica a dama.
- Reinicio de partida.
- Giro del tablero.
- Historial de movimientos en espanol.

## Comandos utiles

Compilar:

```powershell
dotnet build .\TableroAjedrez.sln
```

Ejecutar:

```powershell
dotnet run --project .\TableroAjedrez\TableroAjedrez.csproj
```

## Validacion antes de entregar

Despues de cambios de codigo:

```powershell
dotnet build .\TableroAjedrez.sln
```

Comprobaciones manuales recomendadas:

- La ventana abre correctamente.
- Se puede mover un peon dos casillas desde la posicion inicial.
- No se permite mover una pieza del color contrario.
- El rey no puede quedar en jaque.
- El boton `Nueva partida` reinicia el tablero.
- El boton `Girar tablero` cambia la orientacion sin perder la partida.
