using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ModeloEF;
using ModeloEF.Entidades;

namespace Sitio
{
    public partial class EnviarMensaje : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarCategorias();
            }
        }

        private void CargarCategorias()
        {
            var contexto = ContextoAplicacion.ObtenerContexto();
            ddlCategorias.DataSource = contexto.Categorias.OrderBy(c => c.Nombre).ToList();
            ddlCategorias.DataBind();
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            try
            {
                var contexto = ContextoAplicacion.ObtenerContexto();

                if (!DateTime.TryParseExact(txtCaducidad.Text.Trim(),
                        new[] { "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy" },
                        CultureInfo.CurrentCulture, DateTimeStyles.None, out var caducidad))
                {
                    throw new ArgumentException("La fecha de caducidad no es válida.");
                }

                var remitente = contexto.Usuarios.FirstOrDefault(u => u.Username.Trim() == txtRemitente.Text.Trim());
                if (remitente == null)
                {
                    throw new ArgumentException("El remitente no existe.");
                }

                var categoria = contexto.Categorias.FirstOrDefault(c => c.Codigo.Trim() == ddlCategorias.SelectedValue.Trim());
                if (categoria == null)
                {
                    throw new ArgumentException("La categoría seleccionada no es válida.");
                }

                var destinatariosUsernames = ObtenerDestinatarios();
                var destinatarios = ObtenerUsuariosDestinatarios(contexto, destinatariosUsernames);

                var mensaje = new Mensaje
                {
                    Asunto = txtAsunto.Text.Trim(),
                    Texto = txtTexto.Text.Trim(),
                    FechaEnvio = DateTime.Now,
                    FechaCaducidad = caducidad,
                    Remitente = remitente,
                    Categoria = categoria
                };

                mensaje.ValidarAlta(destinatariosUsernames);

                var nuevoId = contexto.EjecutarAltaMensaje(mensaje);

                foreach (var destino in destinatarios)
                {
                    contexto.EjecutarAltaDestinatario(nuevoId, destino.Username.Trim());
                }

                ContextoAplicacion.Reiniciar();
                lblResultado.Text = $"Mensaje {nuevoId} enviado correctamente.";
                lblResultado.CssClass = "message-success";
            }
            catch (Exception ex)
            {
                lblResultado.Text = ex.Message;
                lblResultado.CssClass = "message-error";
            }
        }

        private IEnumerable<string> ObtenerDestinatarios()
        {
            var lineas = txtDestinatarios.Text
                .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(destino => destino.Trim())
                .Where(destino => !string.IsNullOrWhiteSpace(destino))
                .ToList();

            if (!lineas.Any())
            {
                throw new ArgumentException("Debe indicar al menos un destinatario.");
            }

            return lineas;
        }

        private static List<Usuario> ObtenerUsuariosDestinatarios(BiosMessengerContext contexto, IEnumerable<string> destinatarios)
        {
            var usuarios = new List<Usuario>();

            foreach (var username in destinatarios)
            {
                var usuario = contexto.Usuarios.FirstOrDefault(u => u.Username.Trim() == username);
                if (usuario == null)
                {
                    throw new ArgumentException($"El destinatario {username} no existe.");
                }

                usuarios.Add(usuario);
            }

            return usuarios;
        }
    }
}
