using System;
using EntidadesCompartidas.Entidades;
using Logica.Servicios;

namespace Sitio
{
    public partial class Usuarios : System.Web.UI.Page
    {
        private readonly UsuariosService _usuariosService = new UsuariosService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarUsuarios();
            }
        }

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                var usuario = _usuariosService.Buscar(txtUsername.Text);
                if (usuario == null)
                {
                    MostrarMensaje("Usuario no encontrado.", true);
                    return;
                }

                txtNombre.Text = usuario.NombreCompleto;
                txtEmail.Text = usuario.Email;
                txtFecha.Text = usuario.FechaNacimiento.ToString("dd/MM/yyyy");
                txtPassword.Text = usuario.Pass;
                MostrarMensaje(string.Empty, false);
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message, true);
            }
        }

        protected void btnCrear_Click(object sender, EventArgs e)
        {
            try
            {
                var usuario = ConstruirUsuario(desdePassword: true);
                _usuariosService.Crear(usuario);
                MostrarMensaje("Usuario creado correctamente.", false);
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message, true);
            }
        }

        protected void btnModificar_Click(object sender, EventArgs e)
        {
            try
            {
                var usuario = ConstruirUsuario(desdePassword: false);
                _usuariosService.ModificarPerfil(usuario);
                MostrarMensaje("Usuario modificado correctamente.", false);
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message, true);
            }
        }

        protected void btnPassword_Click(object sender, EventArgs e)
        {
            try
            {
                _usuariosService.ActualizarPassword(txtUsername.Text, txtPassword.Text);
                MostrarMensaje("Contraseña actualizada.", false);
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message, true);
            }
        }

        protected void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                _usuariosService.Eliminar(txtUsername.Text);
                MostrarMensaje("Usuario eliminado.", false);
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message, true);
            }
        }

        private Usuario ConstruirUsuario(bool desdePassword)
        {
            var usuario = new Usuario
            {
                Username = txtUsername.Text?.Trim() ?? string.Empty,
                Pass = desdePassword ? txtPassword.Text : _usuariosService.Buscar(txtUsername.Text)?.Pass ?? txtPassword.Text,
                NombreCompleto = txtNombre.Text,
                Email = txtEmail.Text
            };

            if (DateTime.TryParse(txtFecha.Text, out var fecha))
            {
                usuario.FechaNacimiento = fecha;
            }
            else
            {
                throw new ArgumentException("La fecha debe tener formato válido.");
            }

            return usuario;
        }

        private void CargarUsuarios()
        {
            gvUsuarios.DataSource = _usuariosService.Listar();
            gvUsuarios.DataBind();
        }

        private void MostrarMensaje(string texto, bool esError)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = esError ? "message-error" : "message-success";
        }
    }
}
