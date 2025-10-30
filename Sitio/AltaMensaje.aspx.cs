using System;
using System.Linq;
using ModeloEF.Services;

public partial class AltaMensaje : System.Web.UI.Page
{
    private readonly UsuariosServicio _usuariosServicio = new UsuariosServicio();
    private readonly MensajesServicio _mensajesServicio = new MensajesServicio();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarCombos();
        }
    }

    protected void btnEnviar_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtFechaCaducidad.Text))
            {
                throw new InvalidOperationException("Debe indicar la fecha de caducidad.");
            }

            var fecha = DateTime.Parse(txtFechaCaducidad.Text);
            var destinatarios = lstDestinatarios.GetSelectedIndices()
                .Select(i => lstDestinatarios.Items[i].Value.ToUpperInvariant())
                .ToList();

            _mensajesServicio.CrearMensaje(
                txtAsunto.Text.Trim(),
                txtTexto.Text.Trim(),
                ddlCategoria.SelectedValue,
                ddlRemitente.SelectedValue.ToUpperInvariant(),
                fecha,
                destinatarios);

            lblMensaje.Text = "Mensaje enviado correctamente.";
            txtAsunto.Text = string.Empty;
            txtTexto.Text = string.Empty;
            txtFechaCaducidad.Text = string.Empty;
            lstDestinatarios.ClearSelection();
        }
        catch (Exception ex)
        {
            lblMensaje.Text = ex.Message;
        }
    }

    private void CargarCombos()
    {
        var usuarios = _usuariosServicio.ObtenerTodos();
        ddlRemitente.DataSource = usuarios;
        ddlRemitente.DataTextField = "Username";
        ddlRemitente.DataValueField = "Username";
        ddlRemitente.DataBind();

        lstDestinatarios.DataSource = usuarios;
        lstDestinatarios.DataTextField = "Username";
        lstDestinatarios.DataValueField = "Username";
        lstDestinatarios.DataBind();

        ddlCategoria.DataSource = _mensajesServicio.ObtenerCategorias();
        ddlCategoria.DataTextField = "Nombre";
        ddlCategoria.DataValueField = "Codigo";
        ddlCategoria.DataBind();
    }
}
