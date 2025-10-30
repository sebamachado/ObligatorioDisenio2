using System;
using ModeloEF.Entities;
using ModeloEF.Services;

public partial class ABMUsuarios : System.Web.UI.Page
{
    private readonly UsuariosServicio _usuariosServicio = new UsuariosServicio();

    private string UsuarioSeleccionado
    {
        get => ViewState[nameof(UsuarioSeleccionado)] as string;
        set => ViewState[nameof(UsuarioSeleccionado)] = value;
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
        var username = gvUsuarios.SelectedDataKey?.Value as string;
        if (string.IsNullOrEmpty(username))
        {
            return;
        }

        var usuario = _usuariosServicio.ObtenerPorUsername(username);
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
                _usuariosServicio.Crear(usuario);
                lblMensaje.Text = "Usuario creado correctamente.";
            }
            else
            {
                _usuariosServicio.Actualizar(usuario);
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
            _usuariosServicio.Eliminar(UsuarioSeleccionado);
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
        gvUsuarios.DataSource = _usuariosServicio.ObtenerTodos();
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
