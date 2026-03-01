namespace IPC2_Proyecto1.Models
{
    public enum ResultadoEnfermedad
    {
        SinDeterminar,
        Leve,
        Grave,
        Mortal
    }

    /// <summary>
    /// Representa un paciente con su rejilla inicial y los resultados del análisis
    /// </summary>
    public class Paciente
    {
        public string Nombre { get; set; }
        public int Edad { get; set; }
        public int Periodos { get; set; }   // límite de períodos a evaluar
        public Rejilla RejillaInicial { get; set; }
        public Rejilla RejillaActual { get; set; }
        public int PeriodoActual { get; set; }

        // Resultados del análisis
        public ResultadoEnfermedad Resultado { get; set; }
        public int N { get; set; }   // período en que se repite el patrón inicial o aparece el patrón secundario
        public int N1 { get; set; }  // períodos en que se repite el patrón secundario

        public Paciente(string nombre, int edad, int periodos, Rejilla rejillaInicial)
        {
            Nombre = nombre;
            Edad = edad;
            Periodos = periodos;
            RejillaInicial = rejillaInicial;
            RejillaActual = new Rejilla(rejillaInicial);
            PeriodoActual = 0;
            Resultado = ResultadoEnfermedad.SinDeterminar;
            N = -1;
            N1 = -1;
        }

        public void ReiniciarSimulacion()
        {
            RejillaActual = new Rejilla(RejillaInicial);
            PeriodoActual = 0;
            Resultado = ResultadoEnfermedad.SinDeterminar;
            N = -1;
            N1 = -1;
        }
    }
}
