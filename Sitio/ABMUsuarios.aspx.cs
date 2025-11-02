using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ModeloEF;
using Usuario = ModeloEF.Usuarios;

public partial class ABMUsuarios : System.Web.UI.Page
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

    private string UsuarioSeleccionado
    {
        get
        {
            return ViewState["UsuarioSeleccionado"] as string;
        }
        set
        {
            ViewState["UsuarioSeleccionado"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            CargarUsuarios();
        }
    }

    protected void gvUsuarios_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (gvUsuarios.SelectedDataKey == null || gvUsuarios.SelectedDataKey.Value == null)
        {
            return;
        }

        var username = gvUsuarios.SelectedDataKey.Value as string;
        if (string.IsNullOrEmpty(username))
        {
            return;
        }

        var contexto = Contexto;
        var usuario = contexto.Usuarios.SingleOrDefault(u => u.Username.Trim() == username.Trim());
        if (usuario == null)
        {
            lblMensaje.Text = "No se encontró el usuario seleccionado.";
            return;
        }

        UsuarioSeleccionado = usuario.Username.Trim();
        txtUsuario.Text = usuario.Username.Trim();
        txtUsuario.Enabled = false;
        txtPassword.Text = usuario.Pass.Trim();
        txtNombre.Text = usuario.NombreCompleto;
        txtEmail.Text = usuario.Email;
        txtFechaNacimiento.Text = usuario.FechaNacimiento.ToString("yyyy-MM-dd");
        lblMensaje.Text = string.Empty;
    }

    protected void btnNuevo_Click(object sender, EventArgs e)
    {
        LimpiarFormulario();
    }

    protected void btnGuardar_Click(object sender, EventArgs e)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(txtFechaNacimiento.Text))
            {
                throw new InvalidOperationException("Debe ingresar la fecha de nacimiento.");
            }

            var contexto = Contexto;
            var usuario = new Usuario
            {
                Username = txtUsuario.Text.Trim().ToUpperInvariant(),
                Pass = txtPassword.Text.Trim(),
                NombreCompleto = txtNombre.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                FechaNacimiento = DateTime.Parse(txtFechaNacimiento.Text)
            };

            if (string.IsNullOrEmpty(UsuarioSeleccionado))
            {
                Validador.ValidarUsuario(usuario);

                if (contexto.Usuarios.Any(u => u.Username.Trim() == usuario.Username))
                    throw new InvalidOperationException("Ya existe un usuario con el nombre especificado.");

                contexto.Usuarios.Add(usuario);
                contexto.SaveChanges();
                lblMensaje.Text = "Usuario creado correctamente.";
            }
            else
            {
                Validador.ValidarActualizacionUsuario(usuario);

                var existente = contexto.Usuarios.SingleOrDefault(u => u.Username.Trim() == UsuarioSeleccionado);
                if (existente == null)
                    throw new InvalidOperationException("El usuario indicado no existe.");

                existente.Pass = usuario.Pass;
                existente.NombreCompleto = usuario.NombreCompleto;
                existente.Email = usuario.Email;
                existente.FechaNacimiento = usuario.FechaNacimiento;
                contexto.SaveChanges();
                lblMensaje.Text = "Usuario actualizado correctamente.";
            }

            CargarUsuarios();
            LimpiarFormulario();
        }
        catch (Exception ex)
        {
            lblMensaje.Text = ex.Message;
        }
    }

    protected void btnEliminar_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(UsuarioSeleccionado))
        {
            lblMensaje.Text = "Debe seleccionar un usuario para eliminar.";
            return;
        }

        try
        {
            var contexto = Contexto;
            var usuario = contexto.Usuarios.SingleOrDefault(u => u.Username.Trim() == UsuarioSeleccionado);
            if (usuario == null)
                throw new InvalidOperationException("El usuario indicado no existe.");

            var tieneMensajesEnviados = contexto.Mensajes.Any(m => m.Remitente.Username.Trim() == UsuarioSeleccionado);
            var tieneMensajesRecibidos = contexto.Mensajes.Any(m => m.Destinatarios.Any(d => d.Username.Trim() == UsuarioSeleccionado));

            Validador.ValidarEliminacionUsuario(usuario, tieneMensajesEnviados, tieneMensajesRecibidos);

            var usernameParametro = new SqlParameter("@Username", SqlDbType.Char, 8) { Value = UsuarioSeleccionado };
            var retornoParametro = new SqlParameter("@Ret", SqlDbType.Int) { Direction = ParameterDirection.Output };

            contexto.Database.ExecuteSqlCommand(
                "exec spUsuario_Baja @Username, @Ret output",
                usernameParametro,
                retornoParametro);

            var resultado = (int)retornoParametro.Value;
            if (resultado < 0)
            {
                switch (resultado)
                {
                    case -1:
                        throw new InvalidOperationException("El usuario indicado no existe.");
                    case -2:
                        throw new InvalidOperationException("El usuario tiene mensajes enviados asociados y no puede eliminarse.");
                    case -3:
                        throw new InvalidOperationException("El usuario tiene mensajes recibidos asociados y no puede eliminarse.");
                    default:
                        throw new InvalidOperationException("Se produjo un error al eliminar el usuario.");
                }
            }

            var usuarioLogueado = Session["Usuario"] as Usuario;
            if (usuarioLogueado != null && usuarioLogueado.Username.Trim() == UsuarioSeleccionado)
            {
                Session["Usuario"] = null;
                Response.Redirect("~/Default.aspx");
                return;
            }

            lblMensaje.Text = "Usuario eliminado correctamente.";
            CargarUsuarios();
            LimpiarFormulario();
        }
        catch (Exception ex)
        {
            lblMensaje.Text = ex.Message;
        }
    }

    private void CargarUsuarios()
    {
        var contexto = Contexto;
        gvUsuarios.DataSource = contexto.Usuarios
            .OrderBy(u => u.Username)
            .Select(u => new
            {
                Username = u.Username.Trim(),
                Pass = u.Pass.Trim(),
                u.NombreCompleto,
                u.Email,
                u.FechaNacimiento
            })
            .ToList();
        gvUsuarios.DataBind();
    }

    private void LimpiarFormulario()
    {
        UsuarioSeleccionado = null;
        txtUsuario.Enabled = true;
        txtUsuario.Text = string.Empty;
        txtPassword.Text = string.Empty;
        txtNombre.Text = string.Empty;
        txtEmail.Text = string.Empty;
        txtFechaNacimiento.Text = string.Empty;
    }
}
