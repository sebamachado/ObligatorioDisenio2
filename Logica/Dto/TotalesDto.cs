namespace Logica.Dto
{
    public class TotalesDto
    {
        public int TotalUsuarios { get; set; }
        public int MensajesActivos { get; set; }
        public int MensajesExpirados { get; set; }
        public int MensajesConMasDeCincoDestinatarios { get; set; }
    }
}
