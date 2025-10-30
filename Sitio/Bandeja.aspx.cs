using System;
using Logica.Servicios;

namespace Sitio
{
    public partial class Bandeja : System.Web.UI.Page
    {
        private readonly MensajesService _mensajesService = new MensajesService();

        protected void btnBandeja_Click(object sender, EventArgs e)
        {
            CargarMensajes(() => _mensajesService.ObtenerBandeja(txtUsuario.Text), "Bandeja actualizada.");
        }

        protected void btnEnviados_Click(object sender, EventArgs e)
        {
            CargarMensajes(() => _mensajesService.ObtenerEnviados(txtUsuario.Text), "Mensajes enviados.");
        }

        private void CargarMensajes(Func<System.Collections.IEnumerable> consulta, string mensaje)
        {
            try
            {
                gvMensajes.DataSource = consulta();
                gvMensajes.DataBind();
                lblInfo.Text = mensaje;
                lblInfo.CssClass = "message-success";
            }
            catch (Exception ex)
            {
                lblInfo.Text = ex.Message;
                lblInfo.CssClass = "message-error";
            }
        }
    }
}
