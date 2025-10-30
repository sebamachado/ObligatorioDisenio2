using System;
using Logica.Servicios;

namespace Sitio
{
    public partial class Default : System.Web.UI.Page
    {
        private readonly MensajesService _mensajesService = new MensajesService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarTotales();
            }
        }

        private void CargarTotales()
        {
            try
            {
                var totales = _mensajesService.ObtenerTotales();
                lblTotalUsuarios.Text = totales.TotalUsuarios.ToString();
                lblMensajesActivos.Text = totales.MensajesActivos.ToString();
                lblMensajesVencidos.Text = totales.MensajesExpirados.ToString();
                lblMensajesMasCinco.Text = totales.MensajesConMasDeCincoDestinatarios.ToString();
                lblError.Text = string.Empty;
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
    }
}
