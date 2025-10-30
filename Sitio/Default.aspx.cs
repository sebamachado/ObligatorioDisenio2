using System;
using ModeloEF.Services;

public partial class _Default : System.Web.UI.Page
{
    private readonly UsuariosServicio _usuariosServicio = new UsuariosServicio();
    private readonly MensajesServicio _mensajesServicio = new MensajesServicio();

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarResumen();
        }
    }

    private void CargarResumen()
    {
        try
        {
            lblTotalUsuarios.Text = _usuariosServicio.ContarUsuarios().ToString();
            lblTotalMensajes.Text = _mensajesServicio.ContarMensajes().ToString();
            lblMasDestinatarios.Text = _mensajesServicio.ContarMensajesConMasDeCincoDestinatarios().ToString();
            lblMensaje.Text = string.Empty;
        }
        catch (Exception ex)
        {
            lblMensaje.Text = ex.Message;
        }
    }
}
