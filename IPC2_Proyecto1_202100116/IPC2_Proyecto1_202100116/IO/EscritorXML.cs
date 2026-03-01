using System;
using System.Xml;
using IPC2_Proyecto1.Models;
using IPC2_Proyecto1.Structures;

namespace IPC2_Proyecto1.IO
{
    /// <summary>
    /// Genera el archivo XML de salida con los resultados de cada paciente
    /// </summary>
    public class EscritorXML
    {
        public void GenerarSalida(ListaEnlazada<Paciente> pacientes, string rutaArchivo)
        {
            XmlDocument doc = new XmlDocument();
            XmlDeclaration declaracion = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(declaracion);

            XmlElement raiz = doc.CreateElement("pacientes");
            doc.AppendChild(raiz);

            Nodo<Paciente> actual = pacientes.Cabeza;
            while (actual != null)
            {
                Paciente p = actual.Dato;
                XmlElement nodoPaciente = doc.CreateElement("paciente");

                // Datos personales
                XmlElement datosPers = doc.CreateElement("datospersonales");
                XmlElement nombre = doc.CreateElement("nombre");
                nombre.InnerText = p.Nombre;
                XmlElement edad = doc.CreateElement("edad");
                edad.InnerText = p.Edad.ToString();
                datosPers.AppendChild(nombre);
                datosPers.AppendChild(edad);

                // Periodos
                XmlElement periodos = doc.CreateElement("periodos");
                periodos.InnerText = p.Periodos.ToString();

                // M
                XmlElement mNodo = doc.CreateElement("m");
                mNodo.InnerText = p.RejillaInicial.M.ToString();

                // Resultado
                XmlElement resultado = doc.CreateElement("resultado");
                resultado.InnerText = p.Resultado switch
                {
                    ResultadoEnfermedad.Mortal => "mortal",
                    ResultadoEnfermedad.Grave => "grave",
                    ResultadoEnfermedad.Leve => "leve",
                    _ => "sin determinar"
                };

                nodoPaciente.AppendChild(datosPers);
                nodoPaciente.AppendChild(periodos);
                nodoPaciente.AppendChild(mNodo);
                nodoPaciente.AppendChild(resultado);

                // N y N1 según corresponda
                if (p.N >= 0)
                {
                    XmlElement nNodo = doc.CreateElement("n");
                    nNodo.InnerText = p.N.ToString();
                    nodoPaciente.AppendChild(nNodo);
                }

                if (p.N1 > 1)
                {
                    XmlElement n1Nodo = doc.CreateElement("n1");
                    n1Nodo.InnerText = p.N1.ToString();
                    nodoPaciente.AppendChild(n1Nodo);
                }

                raiz.AppendChild(nodoPaciente);
                actual = actual.Siguiente;
            }

            doc.Save(rutaArchivo);
            Console.WriteLine($"[OK] Archivo de salida generado: {rutaArchivo}");
        }
    }
}
