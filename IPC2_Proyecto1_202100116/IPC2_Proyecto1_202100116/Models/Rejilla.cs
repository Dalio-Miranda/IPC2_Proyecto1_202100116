using IPC2_Proyecto1.Structures;
using System.Text;

namespace IPC2_Proyecto1.Models
{
    /// <summary>
    /// Rejilla cuadrada M x M implementada completamente con listas enlazadas.
    /// NO usa arrays ni diccionarios nativos de C#.
    /// </summary>
    public class Rejilla
    {
        private readonly int m;
        // Lista enlazada de filas, cada fila es una lista enlazada de booleanos
        private ListaEnlazada<ListaEnlazada<bool>> filas;

        public int M => m;
        public int TotalContagiadas => ContarContagiadas();
        public int TotalSanas => (m * m) - TotalContagiadas;
        public ListaEnlazada<Celda> CeldasContagiadas { get; private set; }

        public Rejilla(int m)
        {
            this.m = m;
            filas = new ListaEnlazada<ListaEnlazada<bool>>();
            CeldasContagiadas = new ListaEnlazada<Celda>();

            // Inicializar todas las celdas en false (sanas)
            for (int f = 0; f < m; f++)
            {
                ListaEnlazada<bool> fila = new ListaEnlazada<bool>();
                for (int c = 0; c < m; c++)
                    fila.AgregarFinal(false);
                filas.AgregarFinal(fila);
            }
        }

        /// <summary>Constructor copia</summary>
        public Rejilla(Rejilla origen)
        {
            this.m = origen.m;
            filas = new ListaEnlazada<ListaEnlazada<bool>>();
            CeldasContagiadas = new ListaEnlazada<Celda>();

            // Copiar cada fila
            Nodo<ListaEnlazada<bool>> nodoFila = origen.filas.Cabeza;
            while (nodoFila != null)
            {
                ListaEnlazada<bool> nuevaFila = new ListaEnlazada<bool>();
                Nodo<bool> nodoCol = nodoFila.Dato.Cabeza;
                while (nodoCol != null)
                {
                    nuevaFila.AgregarFinal(nodoCol.Dato);
                    nodoCol = nodoCol.Siguiente;
                }
                filas.AgregarFinal(nuevaFila);
                nodoFila = nodoFila.Siguiente;
            }
            ReconstruirLista();
        }

        /// <summary>Obtiene el valor de una celda (base 0 interna)</summary>
        private bool ObtenerValor(int fila, int col)
        {
            return filas.ObtenerEn(fila).ObtenerEn(col);
        }

        /// <summary>Establece el valor de una celda (base 0 interna)</summary>
        private void EstablecerValor(int fila, int col, bool valor)
        {
            ListaEnlazada<bool> filaLista = filas.ObtenerEn(fila);
            // Recorrer hasta la posición y actualizar
            Nodo<bool> actual = filaLista.Cabeza;
            int indice = 0;
            while (actual != null)
            {
                if (indice == col)
                {
                    actual.Dato = valor;
                    return;
                }
                actual = actual.Siguiente;
                indice++;
            }
        }

        /// <summary>Marca una celda como contagiada (fila y columna en base 1)</summary>
        public void MarcarContagiada(int fila, int columna)
        {
            EstablecerValor(fila - 1, columna - 1, true);
        }

        /// <summary>Devuelve el estado de una celda (base 0) para Graphviz y dibujo</summary>
        public bool ObtenerEstado(int fila, int columna)
        {
            return ObtenerValor(fila, columna);
        }

        private int ContarContagiadas()
        {
            int count = 0;
            Nodo<ListaEnlazada<bool>> nodoFila = filas.Cabeza;
            while (nodoFila != null)
            {
                Nodo<bool> nodoCol = nodoFila.Dato.Cabeza;
                while (nodoCol != null)
                {
                    if (nodoCol.Dato) count++;
                    nodoCol = nodoCol.Siguiente;
                }
                nodoFila = nodoFila.Siguiente;
            }
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
                        if (ObtenerValor(nf, nc)) count++;
                }
            }
            return count;
        }

        /// <summary>
        /// Aplica las reglas de contagio y devuelve la nueva rejilla (siguiente período).
        /// Regla 1: Contagiada con 2 o 3 vecinos → sigue contagiada
        /// Regla 2: Sana con exactamente 3 vecinos → se contagia
        /// </summary>
        public Rejilla SiguientePeriodo()
        {
            Rejilla nueva = new Rejilla(m);
            for (int f = 0; f < m; f++)
            {
                for (int c = 0; c < m; c++)
                {
                    int vecinos = ContarVecinosContagiados(f, c);
                    bool actualContagiada = ObtenerValor(f, c);

                    if (actualContagiada)
                    {
                        // Regla 1
                        if (vecinos == 2 || vecinos == 3)
                            nueva.EstablecerValor(f, c, true);
                    }
                    else
                    {
                        // Regla 2
                        if (vecinos == 3)
                            nueva.EstablecerValor(f, c, true);
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
                    if (ObtenerValor(f, c))
                        CeldasContagiadas.AgregarFinal(new Celda(f + 1, c + 1, true));
        }

        /// <summary>
        /// Genera una firma única del estado actual para comparar patrones.
        /// </summary>
        public string ObtenerFirma()
        {
            StringBuilder sb = new StringBuilder();
            Nodo<ListaEnlazada<bool>> nodoFila = filas.Cabeza;
            while (nodoFila != null)
            {
                Nodo<bool> nodoCol = nodoFila.Dato.Cabeza;
                while (nodoCol != null)
                {
                    sb.Append(nodoCol.Dato ? '1' : '0');
                    nodoCol = nodoCol.Siguiente;
                }
                nodoFila = nodoFila.Siguiente;
            }
            return sb.ToString();
        }
    }
}