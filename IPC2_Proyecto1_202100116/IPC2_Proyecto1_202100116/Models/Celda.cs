namespace IPC2_Proyecto1.Models
{
    /// <summary>
    /// Representa una celda dentro de la rejilla del paciente
    /// </summary>
    public class Celda
    {
        public int Fila { get; set; }
        public int Columna { get; set; }
        public bool EstaContagiada { get; set; }

        public Celda(int fila, int columna, bool estaContagiada = false)
        {
            Fila = fila;
            Columna = columna;
            EstaContagiada = estaContagiada;
        }
    }
}
