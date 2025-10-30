using System.Collections.Generic;

namespace EntidadesCompartidas.Entidades
{
    public class Categoria
    {
        public Categoria()
        {
            Mensajes = new HashSet<Mensaje>();
        }

        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;

        public virtual ICollection<Mensaje> Mensajes { get; protected set; }

        public void Validar() => Validador.ValidarCategoria(this);

        public override string ToString() => $"{Codigo.Trim()} - {Nombre}";
    }
}
