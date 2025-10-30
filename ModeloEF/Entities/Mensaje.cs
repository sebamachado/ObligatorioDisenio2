using System;
using System.Collections.Generic;

namespace ModeloEF.Entities
{
    /// <summary>
    /// Entidad Mensaje asociada a la tabla Mensajes.
    /// </summary>
    public partial class Mensaje
    {
        public Mensaje()
        {
            Destinatarios = new HashSet<Usuario>();
        }

        public int Id { get; set; }
        public string Asunto { get; set; }
        public string Texto { get; set; }
        public DateTime FechaEnvio { get; set; }
        public string RemitenteUsername { get; set; }
        public string CategoriaCod { get; set; }
        public DateTime FechaCaducidad { get; set; }

        public virtual Usuario Remitente { get; set; }
        public virtual Categoria Categoria { get; set; }
        public virtual ICollection<Usuario> Destinatarios { get; set; }
    }
}
