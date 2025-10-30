using System;
using System.Linq;
using Logica.Servicios;

namespace Sitio
{
    public partial class EnviarMensaje : System.Web.UI.Page
    {
        private readonly MensajesService _mensajesService = new MensajesService();
        private readonly CategoriasService _categoriasService = new CategoriasService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarCategorias();
            }
        }

        private void CargarCategorias()
        {
            ddlCategorias.DataSource = _categoriasService.Listar();
            ddlCategorias.DataBind();
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!DateTime.TryParse(txtCaducidad.Text, out var caducidad))
                {
                    throw new ArgumentException("La fecha de caducidad no es válida.");
                }

                var destinatarios = txtDestinatarios.Text
                    .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(d => d.Trim());

                var mensaje = _mensajesService.Enviar(
                    txtRemitente.Text,
                    ddlCategorias.SelectedValue,
                    txtAsunto.Text,
                    txtTexto.Text,
                    caducidad,
                    destinatarios);

                lblResultado.Text = $"Mensaje {mensaje.Id} enviado correctamente.";
                lblResultado.CssClass = "message-success";
            }
            catch (Exception ex)
            {
                lblResultado.Text = ex.Message;
                lblResultado.CssClass = "message-error";
            }
        }
    }
}
