using System;
using System.IO;
using System.Text;
using IPC2_Proyecto1.Models;

namespace IPC2_Proyecto1.Logic
{
    /// <summary>
    /// Genera archivos .dot de Graphviz para visualizar la rejilla de un paciente.
    /// Utiliza HTML-like labels para representar la rejilla como tabla.
    /// </summary>
    public class GeneradorGraphviz
    {
        /// <summary>
        /// Genera el archivo .dot y luego llama a Graphviz para producir el PNG.
        /// </summary>
        public void Generar(Paciente paciente, string carpetaSalida = ".")
        {
            string nombreBase = $"rejilla_{LimpiarNombre(paciente.Nombre)}_periodo{paciente.PeriodoActual}";
            string rutaDot = Path.Combine(carpetaSalida, nombreBase + ".dot");
            string rutaPng = Path.Combine(carpetaSalida, nombreBase + ".png");

            string contenidoDot = GenerarDot(paciente);
            File.WriteAllText(rutaDot, contenidoDot, Encoding.UTF8);

            // Intentar generar el PNG con Graphviz instalado
            try
            {
                var proceso = new System.Diagnostics.Process();
                proceso.StartInfo.FileName = "dot";
                proceso.StartInfo.Arguments = $"-Tpng \"{rutaDot}\" -o \"{rutaPng}\"";
                proceso.StartInfo.UseShellExecute = false;
                proceso.StartInfo.RedirectStandardError = true;
                proceso.Start();
                proceso.WaitForExit();
                if (proceso.ExitCode == 0)
                    Console.WriteLine($"[OK] Imagen generada: {rutaPng}");
                else
                    Console.WriteLine($"[INFO] Archivo .dot generado: {rutaDot} (instala Graphviz para generar PNG)");
            }
            catch
            {
                Console.WriteLine($"[INFO] Archivo .dot generado: {rutaDot}");
                Console.WriteLine("       Graphviz no encontrado. Instálalo y ejecuta:");
                Console.WriteLine($"       dot -Tpng \"{rutaDot}\" -o \"{rutaPng}\"");
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
            sb.AppendLine($"  label=\"Paciente: {paciente.Nombre} | Período: {paciente.PeriodoActual} | Sanas: {r.TotalSanas} | Contagiadas: {r.TotalContagiadas}\";");
            sb.AppendLine("  labelloc=top;");
            sb.AppendLine("  fontsize=14;");
            sb.AppendLine();
            sb.AppendLine("  rejilla [label=<");
            sb.AppendLine("    <TABLE BORDER=\"1\" CELLBORDER=\"1\" CELLSPACING=\"0\" CELLPADDING=\"4\">");

            // Encabezado de columnas
            sb.Append("      <TR><TD BGCOLOR=\"gray\"><B>F\\C</B></TD>");
            for (int c = 1; c <= m; c++)
                sb.Append($"<TD BGCOLOR=\"gray\"><B>{c}</B></TD>");
            sb.AppendLine("</TR>");

            // Filas de la rejilla
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
            // Eliminar caracteres no válidos para nombre de archivo
            StringBuilder sb = new StringBuilder();
            foreach (char c in nombre)
                if (char.IsLetterOrDigit(c) || c == '_')
                    sb.Append(c);
            return sb.Length > 0 ? sb.ToString() : "paciente";
        }
    }
}
