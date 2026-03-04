namespace IPC2_Proyecto1.Structures
{
    /// <summary>
    /// Nodo genérico para lista enlazada simple
    /// </summary>
    public class Nodo<T>
    {
        public T Dato { get; set; }
        public Nodo<T> Siguiente { get; set; }

        public Nodo(T dato)
        {
            Dato = dato;
            Siguiente = null;
        }
    }
}