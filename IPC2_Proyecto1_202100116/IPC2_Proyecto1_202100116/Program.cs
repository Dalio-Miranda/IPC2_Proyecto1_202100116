using System;
using IPC2_Proyecto1.IO;
using IPC2_Proyecto1.Logic;
using IPC2_Proyecto1.Models;
using IPC2_Proyecto1.Structures;

namespace IPC2_Proyecto1
{
    class Program
    {
        // Lista global de pacientes cargados en memoria
        static ListaEnlazada<Paciente> listaPacientes = new ListaEnlazada<Paciente>();
        // Paciente seleccionado actualmente
        static Paciente pacienteActual = null;
        static Simulador simuladorActual = null;

        static void Main(string[] args)
        {
            Console.Title = "IPC2 - Proyecto 1 | Simulación Epidemiológica";
            bool salir = false;

            while (!salir)
            {
                MostrarMenu();
                string opcion = Console.ReadLine()?.Trim();

                switch (opcion)
                {
                    case "1": CargarArchivoXML(); break;
                    case "2": SeleccionarPaciente(); break;
                    case "3": AvanzarUnPeriodo(); break;
                    case "4": EjecutarTodo(); break;
                    case "5": GenerarSalidaXML(); break;
                    case "6": GenerarVisualizacion(); break;
                    case "7": LimpiarMemoria(); break;
                    case "8": salir = true; break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Opción no válida.");
                        Console.ResetColor();
                        break;
                }

                if (!salir)
                {
                    Console.WriteLine("\nPresione ENTER para continuar...");
                    Console.ReadLine();
                }
            }

            Console.WriteLine("¡Hasta luego!");
        }

        static void MostrarMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔══════════════════════════════════════════════════╗");
            Console.WriteLine("║     LABORATORIO EPIDEMIOLÓGICO - IPC2            ║");
            Console.WriteLine("╠══════════════════════════════════════════════════╣");
            Console.ResetColor();

            // Mostrar estado actual
            if (pacienteActual != null)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"  Paciente: {pacienteActual.Nombre} | Período: {pacienteActual.PeriodoActual}");
                Console.WriteLine($"  Sanas: {pacienteActual.RejillaActual.TotalSanas} | Contagiadas: {pacienteActual.RejillaActual.TotalContagiadas}");
                if (pacienteActual.Resultado != ResultadoEnfermedad.SinDeterminar)
                {
                    Console.Write("  Resultado: ");
                    Console.ForegroundColor = pacienteActual.Resultado == ResultadoEnfermedad.Mortal ? ConsoleColor.Red
                        : pacienteActual.Resultado == ResultadoEnfermedad.Grave ? ConsoleColor.DarkYellow
                        : ConsoleColor.Green;
                    Console.Write(pacienteActual.Resultado.ToString().ToUpper());
                    if (pacienteActual.N >= 0) Console.Write($" | N={pacienteActual.N}");
                    if (pacienteActual.N1 > 0) Console.Write($" | N1={pacienteActual.N1}");
                    Console.WriteLine();
                }
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine("  Sin paciente seleccionado.");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╠══════════════════════════════════════════════════╣");
            Console.ResetColor();
            Console.WriteLine("  [1] Cargar archivo XML de entrada");
            Console.WriteLine("  [2] Seleccionar paciente");
            Console.WriteLine("  [3] Avanzar un período");
            Console.WriteLine("  [4] Ejecutar automáticamente (todos los períodos)");
            Console.WriteLine("  [5] Generar archivo XML de salida");
            Console.WriteLine("  [6] Generar visualización Graphviz");
            Console.WriteLine("  [7] Limpiar memoria de pacientes");
            Console.WriteLine("  [8] Salir");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╚══════════════════════════════════════════════════╝");
            Console.ResetColor();
            Console.Write("Opción: ");
        }

        static void CargarArchivoXML()
        {
            Console.Write("Ruta del archivo XML de entrada: ");
            string ruta = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(ruta) || !System.IO.File.Exists(ruta))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Archivo no encontrado.");
                Console.ResetColor();
                return;
            }

            try
            {
                LectorXML lector = new LectorXML();
                ListaEnlazada<Paciente> nuevos = lector.CargarPacientes(ruta);

                Nodo<Paciente> actual = nuevos.Cabeza;
                int count = 0;
                while (actual != null)
                {
                    listaPacientes.AgregarFinal(actual.Dato);
                    actual = actual.Siguiente;
                    count++;
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[OK] {count} paciente(s) cargado(s) correctamente.");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {ex.Message}");
                Console.ResetColor();
            }
        }

        static void SeleccionarPaciente()
        {
            if (listaPacientes.EsVacia)
            {
                Console.WriteLine("No hay pacientes cargados. Cargue primero un archivo XML.");
                return;
            }

            Console.WriteLine("Pacientes disponibles:");
            Nodo<Paciente> actual = listaPacientes.Cabeza;
            int indice = 1;
            while (actual != null)
            {
                string resultado = actual.Dato.Resultado != ResultadoEnfermedad.SinDeterminar
                    ? $" [{actual.Dato.Resultado}]" : "";
                Console.WriteLine($"  [{indice}] {actual.Dato.Nombre} (Edad: {actual.Dato.Edad}){resultado}");
                actual = actual.Siguiente;
                indice++;
            }

            Console.Write("Seleccione número de paciente: ");
            if (!int.TryParse(Console.ReadLine(), out int seleccion) || seleccion < 1 || seleccion > listaPacientes.Tamanio)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Selección no válida.");
                Console.ResetColor();
                return;
            }

            pacienteActual = listaPacientes.ObtenerEn(seleccion - 1);
            pacienteActual.ReiniciarSimulacion();
            simuladorActual = new Simulador(pacienteActual);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] Paciente seleccionado: {pacienteActual.Nombre}");
            Console.ResetColor();
        }

        static void AvanzarUnPeriodo()
        {
            if (pacienteActual == null)
            {
                Console.WriteLine("Primero seleccione un paciente (opción 2).");
                return;
            }
            if (pacienteActual.Resultado != ResultadoEnfermedad.SinDeterminar)
            {
                Console.WriteLine("El análisis ya fue completado para este paciente.");
                return;
            }

            bool terminado = simuladorActual.AvanzarUnPeriodo();
            Console.WriteLine($"Período actual: {pacienteActual.PeriodoActual}");
            Console.WriteLine($"Células sanas: {pacienteActual.RejillaActual.TotalSanas}");
            Console.WriteLine($"Células contagiadas: {pacienteActual.RejillaActual.TotalContagiadas}");

            if (terminado)
            {
                MostrarResultado();
            }
        }

        static void EjecutarTodo()
        {
            if (pacienteActual == null)
            {
                Console.WriteLine("Primero seleccione un paciente (opción 2).");
                return;
            }
            if (pacienteActual.Resultado != ResultadoEnfermedad.SinDeterminar)
            {
                Console.WriteLine("El análisis ya fue completado para este paciente.");
                return;
            }

            Console.WriteLine("Ejecutando simulación...");
            simuladorActual.EjecutarTodo();
            MostrarResultado();
        }

        static void MostrarResultado()
        {
            Console.ForegroundColor = pacienteActual.Resultado == ResultadoEnfermedad.Mortal ? ConsoleColor.Red
                : pacienteActual.Resultado == ResultadoEnfermedad.Grave ? ConsoleColor.DarkYellow
                : ConsoleColor.Green;
            Console.WriteLine($"\n=== RESULTADO: {pacienteActual.Resultado.ToString().ToUpper()} ===");
            if (pacienteActual.N >= 0) Console.WriteLine($"N (período del patrón): {pacienteActual.N}");
            if (pacienteActual.N1 > 0) Console.WriteLine($"N1 (período secundario): {pacienteActual.N1}");
            Console.ResetColor();
        }

        static void GenerarSalidaXML()
        {
            if (listaPacientes.EsVacia)
            {
                Console.WriteLine("No hay pacientes cargados.");
                return;
            }

            Console.Write("Ruta del archivo XML de salida: ");
            string ruta = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(ruta)) ruta = "salida.xml";

            try
            {
                EscritorXML escritor = new EscritorXML();
                escritor.GenerarSalida(listaPacientes, ruta);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ERROR] {ex.Message}");
                Console.ResetColor();
            }
        }

        static void GenerarVisualizacion()
        {
            if (pacienteActual == null)
            {
                Console.WriteLine("Primero seleccione un paciente (opción 2).");
                return;
            }

            Console.Write("Carpeta de salida (ENTER para directorio actual): ");
            string carpeta = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(carpeta)) carpeta = ".";

            GeneradorGraphviz gen = new GeneradorGraphviz();
            gen.Generar(pacienteActual, carpeta);
        }

        static void LimpiarMemoria()
        {
            Console.Write("¿Está seguro que desea limpiar la memoria? (s/n): ");
            string resp = Console.ReadLine()?.Trim().ToLower();
            if (resp == "s")
            {
                listaPacientes.Limpiar();
                pacienteActual = null;
                simuladorActual = null;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[OK] Memoria limpiada.");
                Console.ResetColor();
            }
        }
    }
}
