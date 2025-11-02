using System;
using System.Linq;
using ModeloEF;

public partial class _Default : System.Web.UI.Page
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
            var contexto = Contexto;
            lblTotalUsuarios.Text = contexto.Usuarios.Count().ToString();
            lblTotalMensajes.Text = contexto.Mensajes.Count().ToString();
            lblMasDestinatarios.Text = contexto.Mensajes.Count(m => m.Destinatarios.Count() > 5).ToString();
            lblMensaje.Text = string.Empty;
        }
        catch (Exception ex)
        {
            lblMensaje.Text = ex.Message;
        }
    }
}
