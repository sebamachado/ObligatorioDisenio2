using System.Collections.Generic;

namespace ModeloEF.Entidades
{
    public class Categoria
    {
        public Categoria()
        {
            Mensajes = new HashSet<Mensaje>();
        }

        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;

        public virtual ICollection<Mensaje> Mensajes { get; private set; }

        public override string ToString() => $"{Codigo.Trim()} - {Nombre}";
    }
}
