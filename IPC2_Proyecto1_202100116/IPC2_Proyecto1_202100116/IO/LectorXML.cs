using System;
using System.Xml;
using IPC2_Proyecto1.Models;
using IPC2_Proyecto1.Structures;

namespace IPC2_Proyecto1.IO
{
    /// <summary>
    /// Lee el archivo XML de entrada y construye la lista de pacientes.
    /// Soporta tanto el tag rejilla como rejilla1 por compatibilidad.
    /// </summary>
    public class LectorXML
    {
        public ListaEnlazada<Paciente> CargarPacientes(string rutaArchivo)
        {
            ListaEnlazada<Paciente> pacientes = new ListaEnlazada<Paciente>();

            XmlDocument doc = new XmlDocument();
            doc.Load(rutaArchivo);

            XmlNodeList nodosPaciente = doc.GetElementsByTagName("paciente");

            foreach (XmlNode nodoPaciente in nodosPaciente)
            {
                try
                {
                    string nombre = nodoPaciente.SelectSingleNode("datospersonales/nombre")?.InnerText ?? "Sin nombre";
                    int edad = int.Parse(nodoPaciente.SelectSingleNode("datospersonales/edad")?.InnerText ?? "0");
                    int periodos = int.Parse(nodoPaciente.SelectSingleNode("periodos")?.InnerText ?? "100");
                    int m = int.Parse(nodoPaciente.SelectSingleNode("m")?.InnerText ?? "10");

                    Rejilla rejilla = new Rejilla(m);

                    // Soportar tanto <rejilla> como <rejilla1>
                    XmlNodeList celdas = nodoPaciente.SelectNodes("rejilla/celda");
                    if (celdas == null || celdas.Count == 0)
                        celdas = nodoPaciente.SelectNodes("rejilla1/celda");

                    if (celdas != null)
                    {
                        foreach (XmlNode celdaNodo in celdas)
                        {
                            int fila = int.Parse(celdaNodo.Attributes["f"].Value);
                            int columna = int.Parse(celdaNodo.Attributes["c"].Value);
                            rejilla.MarcarContagiada(fila, columna);
                        }
                    }
                    rejilla.ReconstruirLista();

                    Paciente paciente = new Paciente(nombre, edad, periodos, rejilla);
                    pacientes.AgregarFinal(paciente);
                    Console.WriteLine($"[OK] Paciente cargado: {nombre}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] al cargar paciente: {ex.Message}");
                }
            }

            return pacientes;
        }
    }
}