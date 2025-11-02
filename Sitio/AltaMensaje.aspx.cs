using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ModeloEF;

public partial class AltaMensaje : System.Web.UI.Page
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

            var contexto = Contexto;
            var fecha = DateTime.Parse(txtFechaCaducidad.Text);
            var categoria = ddlCategoria.SelectedValue.Trim().ToUpperInvariant();
            var remitente = ddlRemitente.SelectedValue.Trim().ToUpperInvariant();
            var asunto = txtAsunto.Text.Trim();
            var texto = txtTexto.Text.Trim();

            var destinatarios = ObtenerDestinatariosSeleccionados();

            var usuariosSistema = contexto.Usuarios.ToList();

            var remitenteEntidad = usuariosSistema
                .SingleOrDefault(u => string.Equals(u.Username.Trim(), remitente, StringComparison.OrdinalIgnoreCase));
            if (remitenteEntidad == null)
                throw new InvalidOperationException("El remitente indicado no existe.");

            var destinatariosEntidad = usuariosSistema
                .Where(u => destinatarios.Contains(u.Username.Trim().ToUpperInvariant()))
                .ToList();

            if (destinatariosEntidad.Count != destinatarios.Count)
                throw new InvalidOperationException("Existen destinatarios seleccionados que no se encuentran en el sistema.");

            Validador.ValidarMensaje(asunto, texto, categoria, remitenteEntidad, fecha, destinatariosEntidad);

            var asuntoParametro = new SqlParameter("@Asunto", SqlDbType.VarChar, 50) { Value = asunto };
            var textoParametro = new SqlParameter("@Texto", SqlDbType.VarChar, 100) { Value = texto };
            var categoriaParametro = new SqlParameter("@CategoriaCod", SqlDbType.Char, 3) { Value = categoria };
            var remitenteParametro = new SqlParameter("@Remitente", SqlDbType.Char, 8) { Value = remitente };
            var fechaParametro = new SqlParameter("@FechaCaducidad", SqlDbType.DateTime) { Value = fecha };
            var idParametro = new SqlParameter("@Id", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var retornoParametro = new SqlParameter("@Ret", SqlDbType.Int) { Direction = ParameterDirection.Output };

            contexto.Database.ExecuteSqlCommand(
                "exec spMensaje_Alta @Asunto, @Texto, @CategoriaCod, @Remitente, @FechaCaducidad, @Id output, @Ret output",
                asuntoParametro,
                textoParametro,
                categoriaParametro,
                remitenteParametro,
                fechaParametro,
                idParametro,
                retornoParametro);

            var resultadoAlta = (int)retornoParametro.Value;
            if (resultadoAlta < 0)
                LanzarExcepcionAlta(resultadoAlta);

            var idMensaje = (int)idParametro.Value;

            foreach (var destinatario in destinatariosEntidad)
            {
                var idMensajeParametro = new SqlParameter("@IdMsg", SqlDbType.Int) { Value = idMensaje };
                var destinatarioParametro = new SqlParameter("@Destino", SqlDbType.Char, 8) { Value = destinatario.Username.Trim() };
                var retornoDestinatario = new SqlParameter("@Ret", SqlDbType.Int) { Direction = ParameterDirection.Output };

                contexto.Database.ExecuteSqlCommand(
                    "exec spMensaje_AddDestinatario @IdMsg, @Destino, @Ret output",
                    idMensajeParametro,
                    destinatarioParametro,
                    retornoDestinatario);

                var resultadoDestinatario = (int)retornoDestinatario.Value;
                if (resultadoDestinatario < 0)
                    LanzarExcepcionDestinatario(resultadoDestinatario);
            }

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
        var contexto = Contexto;

        var usuarios = contexto.Usuarios
            .OrderBy(u => u.Username)
            .Select(u => new { Username = u.Username.Trim() })
            .ToList();

        ddlRemitente.DataSource = usuarios;
        ddlRemitente.DataTextField = "Username";
        ddlRemitente.DataValueField = "Username";
        ddlRemitente.DataBind();

        lstDestinatarios.DataSource = usuarios;
        lstDestinatarios.DataTextField = "Username";
        lstDestinatarios.DataValueField = "Username";
        lstDestinatarios.DataBind();

        ddlCategoria.DataSource = contexto.Categorias
            .OrderBy(c => c.Nombre)
            .Select(c => new { Nombre = c.Nombre, Codigo = c.Codigo.Trim() })
            .ToList();
        ddlCategoria.DataTextField = "Nombre";
        ddlCategoria.DataValueField = "Codigo";
        ddlCategoria.DataBind();
    }

    private List<string> ObtenerDestinatariosSeleccionados()
    {
        return lstDestinatarios.GetSelectedIndices()
            .Select(i => lstDestinatarios.Items[i].Value.Trim().ToUpperInvariant())
            .ToList();
    }

    private static void LanzarExcepcionAlta(int codigo)
    {
        switch (codigo)
        {
            case -1:
                throw new InvalidOperationException("El remitente indicado no existe.");
            case -2:
                throw new InvalidOperationException("La categoría indicada no existe.");
            case -3:
                throw new InvalidOperationException("La fecha de caducidad debe ser posterior a la fecha actual.");
            default:
                throw new InvalidOperationException("Se produjo un error al crear el mensaje.");
        }
    }

    private static void LanzarExcepcionDestinatario(int codigo)
    {
        switch (codigo)
        {
            case -1:
                throw new InvalidOperationException("El mensaje indicado no existe.");
            case -2:
                throw new InvalidOperationException("El destinatario indicado no existe.");
            case -3:
                throw new InvalidOperationException("El destinatario ya se encuentra asociado al mensaje.");
            default:
                throw new InvalidOperationException("Se produjo un error al asociar el destinatario al mensaje.");
        }
    }
}
