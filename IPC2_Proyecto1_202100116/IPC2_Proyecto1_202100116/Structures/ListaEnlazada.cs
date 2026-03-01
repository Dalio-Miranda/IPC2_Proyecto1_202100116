namespace IPC2_Proyecto1.Structures
{
    /// <summary>
    /// Lista enlazada simple genérica implementada desde cero
    /// </summary>
    public class ListaEnlazada<T>
    {
        private Nodo<T> cabeza;
        private int tamanio;

        public int Tamanio => tamanio;
        public bool EsVacia => cabeza == null;
        public Nodo<T> Cabeza => cabeza;

        public ListaEnlazada()
        {
            cabeza = null;
            tamanio = 0;
        }

        /// <summary>Agrega un elemento al final de la lista</summary>
        public void AgregarFinal(T dato)
        {
            Nodo<T> nuevo = new Nodo<T>(dato);
            if (cabeza == null)
            {
                cabeza = nuevo;
            }
            else
            {
                Nodo<T> actual = cabeza;
                while (actual.Siguiente != null)
                    actual = actual.Siguiente;
                actual.Siguiente = nuevo;
            }
            tamanio++;
        }

        /// <summary>Agrega un elemento al inicio de la lista</summary>
        public void AgregarInicio(T dato)
        {
            Nodo<T> nuevo = new Nodo<T>(dato);
            nuevo.Siguiente = cabeza;
            cabeza = nuevo;
            tamanio++;
        }

        /// <summary>Obtiene el elemento en la posición indicada (0-based)</summary>
        public T ObtenerEn(int indice)
        {
            if (indice < 0 || indice >= tamanio)
                throw new System.IndexOutOfRangeException($"Índice {indice} fuera de rango.");
            Nodo<T> actual = cabeza;
            for (int i = 0; i < indice; i++)
                actual = actual.Siguiente;
            return actual.Dato;
        }

        /// <summary>Elimina todos los elementos de la lista</summary>
        public void Limpiar()
        {
            cabeza = null;
            tamanio = 0;
        }

        /// <summary>Verifica si la lista contiene un elemento usando Equals</summary>
        public bool Contiene(T dato)
        {
            Nodo<T> actual = cabeza;
            while (actual != null)
            {
                if (actual.Dato.Equals(dato))
                    return true;
                actual = actual.Siguiente;
            }
            return false;
        }
    }
}
