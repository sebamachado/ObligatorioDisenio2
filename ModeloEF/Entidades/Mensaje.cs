using System;
using System.Collections.Generic;
using System.Linq;

namespace ModeloEF.Entidades
{
    public class Mensaje
    {
        public Mensaje()
        {
            Destinatarios = new HashSet<Usuario>();
        }

        public int Id { get; set; }
        public string Asunto { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
        public DateTime FechaCaducidad { get; set; }

        public virtual Usuario Remitente { get; set; }
        public virtual Categoria Categoria { get; set; }
        public virtual ICollection<Usuario> Destinatarios { get; private set; }

        public void ValidarAlta(IEnumerable<string> destinatarios)
        {
            Validador.ValidarMensajeAlta(this, destinatarios);
        }

        public bool EstaVencidoParaDestinatario(string username)
        {
            var hoy = DateTime.Now;
            var remitente = Remitente?.Username?.Trim() ?? string.Empty;
            return hoy > FechaCaducidad && !string.Equals(remitente, username?.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public IEnumerable<string> ObtenerDestinatariosUsernames()
            => Destinatarios.Select(d => d.Username.Trim());
    }
}
