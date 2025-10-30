using System;
using System.Globalization;
using System.Linq;
using ModeloEF;
using ModeloEF.Entidades;

namespace Sitio
{
    public partial class Usuarios : System.Web.UI.Page
    {
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
                var contexto = ContextoAplicacion.ObtenerContexto();
                var usuario = contexto.Usuarios.FirstOrDefault(u => u.Username.Trim() == txtUsername.Text.Trim());

                if (usuario == null)
                {
                    MostrarMensaje("Usuario no encontrado.", true);
                    LimpiarFormulario();
                    return;
                }

                txtNombre.Text = usuario.NombreCompleto;
                txtEmail.Text = usuario.Email;
                txtFecha.Text = usuario.FechaNacimiento.ToString("yyyy-MM-dd");
                txtPassword.Text = usuario.Pass.Trim();
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
                var contexto = ContextoAplicacion.ObtenerContexto();
                var usuario = ConstruirUsuarioDesdeFormulario(contexto, incluirPassword: true);

                usuario.ValidarAlta();
                contexto.Usuarios.Add(usuario);
                contexto.SaveChanges();

                MostrarMensaje("Usuario creado correctamente.", false);
                LimpiarFormulario();
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
                var contexto = ContextoAplicacion.ObtenerContexto();
                var usuario = contexto.Usuarios.FirstOrDefault(u => u.Username.Trim() == txtUsername.Text.Trim());

                if (usuario == null)
                {
                    throw new InvalidOperationException("Debe buscar un usuario existente para modificarlo.");
                }

                ActualizarDatosBasicos(usuario);
                usuario.ValidarModificacion();
                contexto.SaveChanges();

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
                var contexto = ContextoAplicacion.ObtenerContexto();
                var usuario = contexto.Usuarios.FirstOrDefault(u => u.Username.Trim() == txtUsername.Text.Trim());

                if (usuario == null)
                {
                    throw new InvalidOperationException("Debe buscar un usuario existente para cambiar la contraseña.");
                }

                usuario.ValidarCambioPassword(txtPassword.Text);
                contexto.SaveChanges();

                MostrarMensaje("Contraseña actualizada.", false);
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
                var username = txtUsername.Text.Trim();
                if (string.IsNullOrWhiteSpace(username))
                {
                    throw new InvalidOperationException("Debe indicar un usuario para eliminar.");
                }

                var contexto = ContextoAplicacion.ObtenerContexto();
                contexto.EjecutarBajaUsuario(username);

                ContextoAplicacion.Reiniciar();
                MostrarMensaje("Usuario eliminado.", false);
                LimpiarFormulario();
                CargarUsuarios();
            }
            catch (Exception ex)
            {
                MostrarMensaje(ex.Message, true);
            }
        }

        private Usuario ConstruirUsuarioDesdeFormulario(BiosMessengerContext contexto, bool incluirPassword)
        {
            var usuario = new Usuario
            {
                Username = txtUsername.Text.Trim(),
                Pass = incluirPassword ? txtPassword.Text : string.Empty,
                NombreCompleto = txtNombre.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                FechaNacimiento = ObtenerFechaDesdeFormulario()
            };

            if (!incluirPassword)
            {
                var existente = contexto.Usuarios.FirstOrDefault(u => u.Username.Trim() == usuario.Username);
                if (existente != null)
                {
                    usuario.Pass = existente.Pass;
                }
            }

            return usuario;
        }

        private void ActualizarDatosBasicos(Usuario usuario)
        {
            usuario.NombreCompleto = txtNombre.Text.Trim();
            usuario.Email = txtEmail.Text.Trim();
            usuario.FechaNacimiento = ObtenerFechaDesdeFormulario();
        }

        private DateTime ObtenerFechaDesdeFormulario()
        {
            if (!DateTime.TryParse(txtFecha.Text, CultureInfo.CurrentCulture, DateTimeStyles.None, out var fecha)
                && !DateTime.TryParseExact(txtFecha.Text, new[] { "yyyy-MM-dd", "dd/MM/yyyy", "MM/dd/yyyy" }, CultureInfo.CurrentCulture, DateTimeStyles.None, out fecha))
            {
                throw new ArgumentException("La fecha debe tener un formato válido.");
            }

            return fecha;
        }

        private void CargarUsuarios()
        {
            var contexto = ContextoAplicacion.ObtenerContexto();
            gvUsuarios.DataSource = contexto.Usuarios.OrderBy(u => u.Username).ToList();
            gvUsuarios.DataBind();
        }

        private void LimpiarFormulario()
        {
            txtUsername.Text = string.Empty;
            txtNombre.Text = string.Empty;
            txtEmail.Text = string.Empty;
            txtFecha.Text = string.Empty;
            txtPassword.Text = string.Empty;
        }

        private void MostrarMensaje(string texto, bool esError)
        {
            lblMensaje.Text = texto;
            lblMensaje.CssClass = esError ? "message-error" : "message-success";
        }
    }
}
