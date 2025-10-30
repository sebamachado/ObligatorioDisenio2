using System;
using System.Collections.Generic;
using System.Linq;
using ModeloEF;

namespace Sitio
{
    public partial class Bandeja : System.Web.UI.Page
    {
        protected void btnBandeja_Click(object sender, EventArgs e)
        {
            CargarMensajes(ctx => ObtenerBandeja(ctx, txtUsuario.Text.Trim()), "Bandeja actualizada.");
        }

        protected void btnEnviados_Click(object sender, EventArgs e)
        {
            CargarMensajes(ctx => ObtenerEnviados(ctx, txtUsuario.Text.Trim()), "Mensajes enviados.");
        }

        private void CargarMensajes(Func<BiosMessengerContext, IEnumerable<object>> consulta, string mensaje)
        {
            try
            {
                var usuario = txtUsuario.Text.Trim();
                if (string.IsNullOrWhiteSpace(usuario))
                {
                    throw new ArgumentException("Debe indicar el usuario.");
                }

                var contexto = ContextoAplicacion.ObtenerContexto();
                gvMensajes.DataSource = consulta(contexto).ToList();
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

        private static IEnumerable<object> ObtenerBandeja(BiosMessengerContext contexto, string username)
        {
            var mensajes = contexto.Mensajes
                .Where(m => m.Destinatarios.Any(u => u.Username.Trim() == username))
                .ToList()
                .Where(m => !m.EstaVencidoParaDestinatario(username))
                .Select(m => new
                {
                    m.Id,
                    m.Asunto,
                    Remitente = m.Remitente.Username.Trim(),
                    Categoria = m.Categoria.Nombre,
                    m.FechaEnvio,
                    m.FechaCaducidad,
                    Destinatarios = string.Join(", ", m.Destinatarios.Select(d => d.Username.Trim()))
                })
                .OrderByDescending(m => m.FechaEnvio);

            return mensajes;
        }

        private static IEnumerable<object> ObtenerEnviados(BiosMessengerContext contexto, string username)
        {
            var mensajes = contexto.Mensajes
                .Where(m => m.Remitente.Username.Trim() == username)
                .Select(m => new
                {
                    m.Id,
                    m.Asunto,
                    Remitente = m.Remitente.Username.Trim(),
                    Categoria = m.Categoria.Nombre,
                    m.FechaEnvio,
                    m.FechaCaducidad,
                    Destinatarios = string.Join(", ", m.Destinatarios.Select(d => d.Username.Trim()))
                })
                .OrderByDescending(m => m.FechaEnvio);

            return mensajes;
        }
    }
}
