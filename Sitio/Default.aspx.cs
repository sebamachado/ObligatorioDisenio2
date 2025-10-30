using System;
using System.Linq;
using ModeloEF;

namespace Sitio
{
    public partial class Default : System.Web.UI.Page
    {
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
                var contexto = ContextoAplicacion.ObtenerContexto();
                var hoy = DateTime.Now;

                lblTotalUsuarios.Text = contexto.Usuarios.Count().ToString();
                lblMensajesActivos.Text = contexto.Mensajes.Count(m => m.FechaCaducidad >= hoy).ToString();
                lblMensajesVencidos.Text = contexto.Mensajes.Count(m => m.FechaCaducidad < hoy).ToString();

                var mensajesMasDeCinco = (from mensaje in contexto.Mensajes
                                          where (from coincidencia in contexto.Mensajes
                                                 where coincidencia.Id == mensaje.Id
                                                 from destinatario in coincidencia.Destinatarios
                                                 select destinatario.Username).Count() > 5
                                          select mensaje.Id).Count();

                lblMensajesMasCinco.Text = mensajesMasDeCinco.ToString();
                lblError.Text = string.Empty;
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }
    }
}
