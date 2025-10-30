using System;
using System.Linq;
using ModeloEF.Services;

public partial class Bandeja : System.Web.UI.Page
{
    private readonly UsuariosServicio _usuariosServicio = new UsuariosServicio();
    private readonly MensajesServicio _mensajesServicio = new MensajesServicio();

    private string Tipo => (Request.QueryString["tipo"] ?? "entrada").ToLowerInvariant();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ConfigurarTitulo();
            CargarUsuarios();
            CargarMensajes();
        }
    }

    protected void ddlUsuario_SelectedIndexChanged(object sender, EventArgs e)
    {
        CargarMensajes();
    }

    private void ConfigurarTitulo()
    {
        lblTitulo.Text = Tipo == "salida" ? "Bandeja de salida" : "Bandeja de entrada";
    }

    private void CargarUsuarios()
    {
        var usuarios = _usuariosServicio.ObtenerTodos();
        ddlUsuario.DataSource = usuarios;
        ddlUsuario.DataTextField = "Username";
        ddlUsuario.DataValueField = "Username";
        ddlUsuario.DataBind();
    }

    private void CargarMensajes()
    {
        if (ddlUsuario.Items.Count == 0)
        {
            gvMensajes.DataSource = Enumerable.Empty<object>();
            gvMensajes.DataBind();
            return;
        }

        try
        {
            var usuario = ddlUsuario.SelectedValue;
            var datos = Tipo == "salida"
                ? _mensajesServicio.ObtenerBandejaSalida(usuario)
                : _mensajesServicio.ObtenerBandejaEntrada(usuario);

            gvMensajes.DataSource = datos;
            gvMensajes.DataBind();
            lblMensaje.Text = datos.Any() ? string.Empty : "No hay mensajes para mostrar.";
        }
        catch (Exception ex)
        {
            lblMensaje.Text = ex.Message;
        }
    }
}
