using System;
using System.Collections.Generic;

namespace ModeloEF.Entidades
{
    public class Usuario
    {
        public Usuario()
        {
            MensajesEnviados = new HashSet<Mensaje>();
            MensajesRecibidos = new HashSet<Mensaje>();
        }

        public string Username { get; set; } = string.Empty;
        public string Pass { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Email { get; set; } = string.Empty;

        public virtual ICollection<Mensaje> MensajesEnviados { get; private set; }
        public virtual ICollection<Mensaje> MensajesRecibidos { get; private set; }

        public void ValidarAlta() => Validador.ValidarUsuarioAlta(this);

        public void ValidarModificacion() => Validador.ValidarUsuarioModificar(this);

        public void ValidarCambioPassword(string nuevaPassword)
        {
            Validador.ValidarPassword(nuevaPassword);
            Pass = nuevaPassword;
        }

        public override string ToString() => $"{Username.Trim()} - {NombreCompleto}";
    }
}
