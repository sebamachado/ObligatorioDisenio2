using System;
using System.Collections.Generic;
using System.Linq;

namespace EntidadesCompartidas.Entidades
{
    public class Mensaje
    {
        public Mensaje()
        {
            DestinatariosInternos = new HashSet<MensajeDestinatario>();
        }

        public int Id { get; set; }
        public string Asunto { get; set; } = string.Empty;
        public string Texto { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; }
        public DateTime FechaCaducidad { get; set; }

        public string RemitenteUsername { get; set; } = string.Empty;
        public string CategoriaCod { get; set; } = string.Empty;

        public virtual Usuario? Remitente { get; set; }
        public virtual Categoria? Categoria { get; set; }
        public virtual ICollection<MensajeDestinatario> DestinatariosInternos { get; protected set; }

        public IEnumerable<Usuario> Destinatarios => DestinatariosInternos
            .Select(d => d.Destinatario)
            .OfType<Usuario>();

        public void ValidarAlta(IEnumerable<string> destinatarios)
        {
            Validador.ValidarMensajeAlta(this, destinatarios);
        }

        public bool EstaVencidoParaDestinatario(string username)
        {
            var hoy = DateTime.Now;
            return hoy > FechaCaducidad && !string.Equals(RemitenteUsername?.Trim(), username?.Trim(), StringComparison.OrdinalIgnoreCase);
        }
    }
}
