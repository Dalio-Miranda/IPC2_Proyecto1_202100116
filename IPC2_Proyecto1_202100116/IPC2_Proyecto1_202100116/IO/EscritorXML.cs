using IPC2_Proyecto1.Models;
using IPC2_Proyecto1.Structures;
using System.Xml;

using System;
using System.Xml;
using IPC2_Proyecto1.Models;
using IPC2_Proyecto1.Structures;

namespace IPC2_Proyecto1.IO
{
    /// <summary>
    /// Genera el archivo XML de salida con los resultados de cada paciente.
    /// Formato exacto según el enunciado del proyecto.
    /// 
    /// Casos:
    /// - Leve:  solo resultado
    /// - Grave (patron inicial): resultado + n
    /// - Mortal (patron inicial N=1): resultado + n=1
    /// - Grave (patron secundario): resultado + n + n1
    /// - Mortal (patron secundario N1=1): resultado + n + n1=1
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
                nodoPaciente.AppendChild(datosPers);

                // Periodos
                XmlElement periodos = doc.CreateElement("periodos");
                periodos.InnerText = p.Periodos.ToString();
                nodoPaciente.AppendChild(periodos);

                // M
                XmlElement mNodo = doc.CreateElement("m");
                mNodo.InnerText = p.RejillaInicial.M.ToString();
                nodoPaciente.AppendChild(mNodo);

                // Resultado
                XmlElement resultado = doc.CreateElement("resultado");
                resultado.InnerText = p.Resultado switch
                {
                    ResultadoEnfermedad.Mortal => "mortal",
                    ResultadoEnfermedad.Grave => "grave",
                    ResultadoEnfermedad.Leve => "leve",
                    _ => "sin determinar"
                };
                nodoPaciente.AppendChild(resultado);

                // Agregar N y N1 según el caso
                if (p.Resultado == ResultadoEnfermedad.Leve)
                {
                    // Caso leve: no se agrega N ni N1
                }
                else if (p.N1 == -1)
                {
                    // Caso a: patrón inicial se repite → solo N
                    XmlElement nNodo = doc.CreateElement("n");
                    nNodo.InnerText = p.N.ToString();
                    nodoPaciente.AppendChild(nNodo);
                }
                else
                {
                    // Caso b: patrón secundario → N y N1
                    XmlElement nNodo = doc.CreateElement("n");
                    nNodo.InnerText = p.N.ToString();
                    nodoPaciente.AppendChild(nNodo);

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