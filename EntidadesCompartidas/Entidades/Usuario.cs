using System;
using System.Collections.Generic;

namespace EntidadesCompartidas.Entidades
{
    public class Usuario
    {
        public Usuario()
        {
            MensajesEnviados = new HashSet<Mensaje>();
            MensajesRecibidos = new HashSet<MensajeDestinatario>();
        }

        public string Username { get; set; } = string.Empty;
        public string Pass { get; set; } = string.Empty;
        public string NombreCompleto { get; set; } = string.Empty;
        public DateTime FechaNacimiento { get; set; }
        public string Email { get; set; } = string.Empty;

        public virtual ICollection<Mensaje> MensajesEnviados { get; protected set; }
        public virtual ICollection<MensajeDestinatario> MensajesRecibidos { get; protected set; }

        public void ValidarAlta() => Validador.ValidarUsuarioAlta(this);

        public void ValidarActualizacionPassword(string nuevaPassword)
        {
            Validador.ValidarPassword(nuevaPassword);
            Pass = nuevaPassword;
        }

        public void ValidarModificacion() => Validador.ValidarUsuarioModificar(this);

        public override string ToString() => $"{Username.Trim()} - {NombreCompleto}";
    }
}
