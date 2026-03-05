using IPC2_Proyecto1.Models;
using IPC2_Proyecto1.Structures;

namespace IPC2_Proyecto1.Logic
{
    /// <summary>
    /// Simulador de períodos. Detecta repetición de patrones y clasifica la enfermedad.
    /// 
    /// Casos posibles según el enunciado:
    /// a) El patrón INICIAL se repite luego de N períodos:
    ///    - Si N=1 → MORTAL, se guarda N=1
    ///    - Si N>1 → GRAVE, se guarda N=valor
    /// 
    /// b) Un patrón DISTINTO al inicial se repite cada N1 períodos (apareció en período N):
    ///    - Si N1=1 → MORTAL, se guardan N y N1
    ///    - Si N1>1 → GRAVE, se guardan N y N1
    /// </summary>
    public class Simulador
    {
        private Paciente paciente;
        // Lista enlazada que guarda las firmas de cada período (historial)
        // El índice en la lista = número de período
        private ListaEnlazada<string> historialFirmas;

        public Simulador(Paciente paciente)
        {
            this.paciente = paciente;
            historialFirmas = new ListaEnlazada<string>();
            // Guardar firma del estado inicial (período 0)
            historialFirmas.AgregarFinal(paciente.RejillaActual.ObtenerFirma());
        }

        /// <summary>
        /// Avanza UN período. Devuelve true si ya se determinó el resultado.
        /// </summary>
        public bool AvanzarUnPeriodo()
        {
            if (paciente.Resultado != ResultadoEnfermedad.SinDeterminar)
                return true;

            if (paciente.PeriodoActual >= paciente.Periodos)
            {
                paciente.Resultado = ResultadoEnfermedad.Leve;
                return true;
            }

            // Calcular siguiente período
            paciente.RejillaActual = paciente.RejillaActual.SiguientePeriodo();
            paciente.PeriodoActual++;

            string firmaActual = paciente.RejillaActual.ObtenerFirma();

            // Buscar si esta firma ya existió antes
            int indicePrevio = BuscarFirmaEnHistorial(firmaActual);

            if (indicePrevio >= 0)
            {
                // El patrón actual ya se vio en el período indicePrevio
                int n1 = paciente.PeriodoActual - indicePrevio;

                if (indicePrevio == 0)
                {
                    // Caso a: el patrón INICIAL se repite
                    // N = períodos necesarios para repetir el patrón inicial
                    paciente.N = paciente.PeriodoActual;
                    paciente.N1 = -1; // No aplica N1 en este caso

                    if (paciente.N == 1)
                        paciente.Resultado = ResultadoEnfermedad.Mortal;
                    else
                        paciente.Resultado = ResultadoEnfermedad.Grave;
                }
                else
                {
                    // Caso b: un patrón DISTINTO al inicial se repite
                    // N = período en que apareció ese patrón por primera vez
                    // N1 = cada cuántos períodos se repite
                    paciente.N = indicePrevio;
                    paciente.N1 = n1;

                    if (n1 == 1)
                        paciente.Resultado = ResultadoEnfermedad.Mortal;
                    else
                        paciente.Resultado = ResultadoEnfermedad.Grave;
                }

                historialFirmas.AgregarFinal(firmaActual);
                return true;
            }

            historialFirmas.AgregarFinal(firmaActual);

            // Verificar límite
            if (paciente.PeriodoActual >= paciente.Periodos)
            {
                paciente.Resultado = ResultadoEnfermedad.Leve;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Ejecuta todos los períodos hasta determinar el resultado o llegar al límite.
        /// </summary>
        public void EjecutarTodo()
        {
            while (!AvanzarUnPeriodo()) { }
        }

        /// <summary>
        /// Busca la firma en el historial. Devuelve el índice (período) o -1 si no existe.
        /// </summary>
        private int BuscarFirmaEnHistorial(string firma)
        {
            Nodo<string> actual = historialFirmas.Cabeza;
            int indice = 0;
            while (actual != null)
            {
                if (actual.Dato == firma)
                    return indice;
                actual = actual.Siguiente;
                indice++;
            }
            return -1;
        }

        public void ResetearHistorial()
        {
            historialFirmas.Limpiar();
            historialFirmas.AgregarFinal(paciente.RejillaActual.ObtenerFirma());
        }
    }
}