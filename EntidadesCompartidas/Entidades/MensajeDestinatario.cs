namespace EntidadesCompartidas.Entidades
{
    public class MensajeDestinatario
    {
        public int MensajeId { get; set; }
        public string DestinoUsername { get; set; } = string.Empty;

        public virtual Mensaje? Mensaje { get; set; }
        public virtual Usuario? Destinatario { get; set; }
    }
}
