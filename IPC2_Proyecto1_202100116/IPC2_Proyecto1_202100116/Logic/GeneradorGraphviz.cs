using System;
using System.IO;
using System.Text;
using IPC2_Proyecto1.Models;

namespace IPC2_Proyecto1.Logic
{
    /// <summary>
    /// Genera archivos .dot de Graphviz para visualizar la rejilla de un paciente.
    /// </summary>
    public class GeneradorGraphviz
    {
        private const string RutaDot = @"C:\Program Files\Graphviz\bin\dot.exe";

        public void Generar(Paciente paciente, string carpetaSalida = ".")
        {
            // Crear carpeta si no existe
            if (!Directory.Exists(carpetaSalida))
                Directory.CreateDirectory(carpetaSalida);

            string nombreBase = $"rejilla_{LimpiarNombre(paciente.Nombre)}_periodo{paciente.PeriodoActual}";
            string rutaDot = Path.Combine(carpetaSalida, nombreBase + ".dot");
            string rutaPng = Path.Combine(carpetaSalida, nombreBase + ".png");

            string contenidoDot = GenerarDot(paciente);
            File.WriteAllText(rutaDot, contenidoDot, new System.Text.UTF8Encoding(false));
            Console.WriteLine($"[OK] Archivo .dot generado: {rutaDot}");

            if (!File.Exists(RutaDot))
            {
                Console.WriteLine($"[ERROR] No se encontro Graphviz en: {RutaDot}");
                return;
            }

            try
            {
                var proceso = new System.Diagnostics.Process();
                proceso.StartInfo.FileName = RutaDot;
                proceso.StartInfo.Arguments = $"-Tpng \"{rutaDot}\" -o \"{rutaPng}\"";
                proceso.StartInfo.UseShellExecute = false;
                proceso.StartInfo.RedirectStandardError = true;
                proceso.StartInfo.RedirectStandardOutput = true;
                proceso.StartInfo.CreateNoWindow = true;
                proceso.Start();
                proceso.WaitForExit();

                string errOutput = proceso.StandardError.ReadToEnd();

                if (File.Exists(rutaPng))
                {
                    Console.WriteLine($"[OK] Imagen PNG generada: {rutaPng}");
                    // Abrir automaticamente
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = rutaPng,
                        UseShellExecute = true
                    });
                }
                else
                {
                    Console.WriteLine($"[ERROR] No se genero el PNG.");
                    if (!string.IsNullOrEmpty(errOutput))
                        Console.WriteLine($"        Detalle: {errOutput}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
        }

        private string GenerarDot(Paciente paciente)
        {
            Rejilla r = paciente.RejillaActual;
            int m = r.M;
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("digraph Rejilla {");
            sb.AppendLine("  rankdir=TB;");
            sb.AppendLine("  node [shape=none, margin=0];");
            sb.AppendLine($"  label=\"Paciente: {LimpiarNombre(paciente.Nombre)} | Periodo: {paciente.PeriodoActual} | Sanas: {r.TotalSanas} | Contagiadas: {r.TotalContagiadas}\";");
            sb.AppendLine("  labelloc=top;");
            sb.AppendLine("  fontsize=14;");
            sb.AppendLine();
            sb.AppendLine("  rejilla [label=<");
            sb.AppendLine("    <TABLE BORDER=\"1\" CELLBORDER=\"1\" CELLSPACING=\"0\" CELLPADDING=\"4\">");

            // Encabezado columnas
            sb.Append("      <TR><TD BGCOLOR=\"gray\"><B>F/C</B></TD>");
            for (int c = 1; c <= m; c++)
                sb.Append($"<TD BGCOLOR=\"gray\"><B>{c}</B></TD>");
            sb.AppendLine("</TR>");

            // Filas
            for (int f = 0; f < m; f++)
            {
                sb.Append($"      <TR><TD BGCOLOR=\"gray\"><B>{f + 1}</B></TD>");
                for (int c = 0; c < m; c++)
                {
                    string color = r.ObtenerEstado(f, c) ? "#4472C4" : "white";
                    sb.Append($"<TD BGCOLOR=\"{color}\"> </TD>");
                }
                sb.AppendLine("</TR>");
            }

            sb.AppendLine("    </TABLE>");
            sb.AppendLine("  >];");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private string LimpiarNombre(string nombre)
        {
            nombre = nombre.Replace("é", "e").Replace("á", "a")
                           .Replace("í", "i").Replace("ó", "o")
                           .Replace("ú", "u").Replace("ñ", "n")
                           .Replace("É", "E").Replace("Á", "A")
                           .Replace("Í", "I").Replace("Ó", "O")
                           .Replace("Ú", "U").Replace("Ñ", "N")
                           .Replace("ü", "u").Replace("Ü", "U");
            StringBuilder sb = new StringBuilder();
            foreach (char c in nombre)
            {
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);
                else if (c == ' ')
                    sb.Append('_');
            }
            return sb.Length > 0 ? sb.ToString() : "paciente";
        }
    }
}