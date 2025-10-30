using System.Collections.Generic;

namespace ModeloEF.Entities
{
    /// <summary>
    /// Entidad Categoria según la base de datos.
    /// </summary>
    public partial class Categoria
    {
        public Categoria()
        {
            Mensajes = new HashSet<Mensaje>();
        }

        public string Codigo { get; set; }
        public string Nombre { get; set; }

        public virtual ICollection<Mensaje> Mensajes { get; set; }
    }
}
