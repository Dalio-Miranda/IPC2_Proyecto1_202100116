using IPC2_Proyecto1.Structures;
using System.Text;

namespace IPC2_Proyecto1.Models
{
    /// <summary>
    /// Rejilla cuadrada M x M que almacena las celdas en una lista enlazada.
    /// Usa un arreglo 2D internamente para acceso O(1), pero expone también
    /// la lista enlazada para cumplir el requisito de TDA.
    /// </summary>
    public class Rejilla
    {
        private readonly int m;
        private readonly bool[,] estado; // true = contagiada
        public ListaEnlazada<Celda> CeldasContagiadas { get; private set; }

        public int M => m;
        public int TotalContagiadas => ContarContagiadas();
        public int TotalSanas => (m * m) - TotalContagiadas;

        public Rejilla(int m)
        {
            this.m = m;
            estado = new bool[m, m];
            CeldasContagiadas = new ListaEnlazada<Celda>();
        }

        /// <summary>Constructor copia</summary>
        public Rejilla(Rejilla origen)
        {
            this.m = origen.m;
            estado = new bool[m, m];
            CeldasContagiadas = new ListaEnlazada<Celda>();
            for (int f = 0; f < m; f++)
                for (int c = 0; c < m; c++)
                    estado[f, c] = origen.estado[f, c];
            ReconstruirLista();
        }

        /// <summary>Marca una celda como contagiada (fila y columna en base 1)</summary>
        public void MarcarContagiada(int fila, int columna)
        {
            estado[fila - 1, columna - 1] = true;
        }

        public bool EstaContagiada(int fila, int columna)
        {
            return estado[fila - 1, columna - 1];
        }

        private int ContarContagiadas()
        {
            int count = 0;
            for (int f = 0; f < m; f++)
                for (int c = 0; c < m; c++)
                    if (estado[f, c]) count++;
            return count;
        }

        /// <summary>
        /// Cuenta vecinos contagiados de la celda (fila, col) en base 0.
        /// Considera las 8 celdas adyacentes.
        /// </summary>
        public int ContarVecinosContagiados(int fila, int col)
        {
            int count = 0;
            for (int df = -1; df <= 1; df++)
            {
                for (int dc = -1; dc <= 1; dc++)
                {
                    if (df == 0 && dc == 0) continue;
                    int nf = fila + df;
                    int nc = col + dc;
                    if (nf >= 0 && nf < m && nc >= 0 && nc < m)
                        if (estado[nf, nc]) count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Aplica las reglas de contagio y devuelve la nueva rejilla (siguiente período)
        /// Regla 1: Contagiada con 2 o 3 vecinos contagiados → sigue contagiada
        /// Regla 2: Sana con exactamente 3 vecinos contagiados → se contagia
        /// </summary>
        public Rejilla SiguientePeriodo()
        {
            Rejilla nueva = new Rejilla(m);
            for (int f = 0; f < m; f++)
            {
                for (int c = 0; c < m; c++)
                {
                    int vecinos = ContarVecinosContagiados(f, c);
                    if (estado[f, c])
                    {
                        // Regla 1
                        if (vecinos == 2 || vecinos == 3)
                            nueva.estado[f, c] = true;
                    }
                    else
                    {
                        // Regla 2
                        if (vecinos == 3)
                            nueva.estado[f, c] = true;
                    }
                }
            }
            nueva.ReconstruirLista();
            return nueva;
        }

        /// <summary>Reconstruye la lista enlazada de celdas contagiadas</summary>
        public void ReconstruirLista()
        {
            CeldasContagiadas.Limpiar();
            for (int f = 0; f < m; f++)
                for (int c = 0; c < m; c++)
                    if (estado[f, c])
                        CeldasContagiadas.AgregarFinal(new Celda(f + 1, c + 1, true));
        }

        /// <summary>
        /// Genera una firma única del estado actual para comparar patrones.
        /// Usa StringBuilder para no depender de estructuras nativas.
        /// </summary>
        public string ObtenerFirma()
        {
            StringBuilder sb = new StringBuilder();
            for (int f = 0; f < m; f++)
                for (int c = 0; c < m; c++)
                    sb.Append(estado[f, c] ? '1' : '0');
            return sb.ToString();
        }

        /// <summary>Devuelve el estado interno para Graphviz</summary>
        public bool ObtenerEstado(int fila, int columna)
        {
            return estado[fila, columna];
        }
    }
}
