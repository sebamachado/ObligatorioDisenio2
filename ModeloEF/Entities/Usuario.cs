using System;
using System.Collections.Generic;

namespace ModeloEF.Entities
{
    /// <summary>
    /// Entidad Usuario generada a partir del esquema existente.
    /// </summary>
    public partial class Usuario
    {
        public Usuario()
        {
            MensajesEnviados = new HashSet<Mensaje>();
            MensajesDestinados = new HashSet<Mensaje>();
        }

        public string Username { get; set; }
        public string Pass { get; set; }
        public string NombreCompleto { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Email { get; set; }

        public virtual ICollection<Mensaje> MensajesEnviados { get; set; }
        public virtual ICollection<Mensaje> MensajesDestinados { get; set; }
    }
}
