using System;
using System.Data.Entity;
using System.Linq;
using ModeloEF;

public partial class Bandeja : System.Web.UI.Page
{
    private BiosMessenger Contexto
    {
        get
        {
            if (Application["Micontexto"] == null)
                Application["Micontexto"] = new BiosMessenger();

            return Application["Micontexto"] as BiosMessenger;
        }
    }

    private string Tipo
    {
        get
        {
            string tipo = Request.QueryString["tipo"];
            if (tipo == null)
            {
                tipo = "entrada";
            }

            return tipo.ToLowerInvariant();
        }
    }

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
        var contexto = Contexto;
        var usuarios = contexto.Usuarios
            .OrderBy(u => u.Username)
            .Select(u => new { Username = u.Username.Trim() })
            .ToList();

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
            var contexto = Contexto;
            var usuario = ddlUsuario.SelectedValue.Trim();
            var datos = Tipo == "salida"
                ? contexto.Mensajes
                    .Include(m => m.Destinatarios)
                    .Include(m => m.Remitente)
                    .Include(m => m.Categorias)
                    .Where(m => m.Remitente.Username.Trim() == usuario)
                    .OrderByDescending(m => m.FechaEnvio)
                    .ToList()
                : contexto.Mensajes
                    .Include(m => m.Destinatarios)
                    .Include(m => m.Remitente)
                    .Include(m => m.Categorias)
                    .Where(m => m.Destinatarios.Any(d => d.Username.Trim() == usuario) && m.FechaCaducidad > DateTime.Now)
                    .OrderByDescending(m => m.FechaEnvio)
                    .ToList();

            gvMensajes.DataSource = datos.Select(m => new
            {
                m.Asunto,
                Remitente = m.Remitente.Username.Trim(),
                Categoria = m.Categorias.Nombre,
                FechaEnvio = m.FechaEnvio,
                FechaCaducidad = m.FechaCaducidad,
                Destinatarios = string.Join(", ", m.Destinatarios.Select(d => d.Username.Trim()))
            });
            gvMensajes.DataBind();
            lblMensaje.Text = datos.Any() ? string.Empty : "No hay mensajes para mostrar.";
        }
        catch (Exception ex)
        {
            lblMensaje.Text = ex.Message;
        }
    }
}
