using System;
using System.Collections.Generic;
using System.Linq;
using Usuario = ModeloEF.Usuarios;

namespace ModeloEF
{
    //esta clase esta por fuera del modelo EF - significa que si actualizo el modelo no la toca
    //pero esta dentro de la misma biblioteca

    public class Validador
    {
        //genero operaciones de clase para validar cada concepto
        //son operaciones VOID - lanzan exception si hay error (ya viene objeto cargado con los datos)
        public static void ValidarUsuario(Usuario usuario)
        {
            if (usuario == null)
                throw new Exception("Usuario: datos obligatorios");

            var nombre = usuario.Username == null ? string.Empty : usuario.Username.Trim();
            if (nombre.Length != 8)
                throw new Exception("Usuario: nombre debe tener 8 caracteres");

            if (nombre.ToUpperInvariant() != nombre)
                throw new Exception("Usuario: nombre en mayúsculas");

            if (nombre.Any(caracter => !char.IsLetterOrDigit(caracter)))
                throw new Exception("Usuario: nombre solo admite letras y números");

            var pass = usuario.Pass == null ? string.Empty : usuario.Pass.Trim();
            if (pass.Length != 8)
                throw new Exception("Usuario: password debe tener 8 caracteres");

            if (!pass.Any(char.IsUpper))
                throw new Exception("Usuario: password requiere mayúsculas");

            if (!pass.Any(char.IsLower))
                throw new Exception("Usuario: password requiere minúsculas");

            if (!pass.Any(char.IsDigit))
                throw new Exception("Usuario: password requiere números");

            if (string.IsNullOrWhiteSpace(usuario.NombreCompleto) || usuario.NombreCompleto.Trim().Length > 50)
                throw new Exception("Usuario: nombre completo obligatorio (máx. 50 caracteres)");

            if (string.IsNullOrWhiteSpace(usuario.Email) || usuario.Email.Trim().Length > 100)
                throw new Exception("Usuario: email obligatorio (máx. 100 caracteres)");

            if (!usuario.Email.Contains("@") || !usuario.Email.Contains("."))
                throw new Exception("Usuario: email con formato inválido");

            if (usuario.FechaNacimiento.Date >= DateTime.Today)
                throw new Exception("Usuario: fecha de nacimiento en el pasado");

            if (CalcularEdad(usuario.FechaNacimiento, DateTime.Today) < 18)
                throw new Exception("Usuario: mayor de edad");
        }

        public static void ValidarActualizacionUsuario(Usuario usuario)
        {
            ValidarUsuario(usuario);
        }

        public static void ValidarEliminacionUsuario(Usuario usuario, bool tieneMensajesEnviados, bool tieneMensajesRecibidos)
        {
            if (usuario == null)
                throw new Exception("Usuario: datos obligatorios");

            if (tieneMensajesEnviados)
                throw new Exception("Usuario: no se puede eliminar con mensajes enviados");

            if (tieneMensajesRecibidos)
                throw new Exception("Usuario: no se puede eliminar con mensajes recibidos");
        }

        public static void ValidarMensaje(string asunto, string texto, string categoriaCod, Usuario remitente, DateTime fechaCaducidad, IEnumerable<Usuario> destinatarios)
        {
            if (remitente == null)
                throw new Exception("Mensaje: remitente obligatorio");

            var asuntoLimpio = string.IsNullOrWhiteSpace(asunto) ? string.Empty : asunto.Trim();
            if (asuntoLimpio.Length == 0 || asuntoLimpio.Length > 50)
                throw new Exception("Mensaje: asunto obligatorio (máx. 50 caracteres)");

            var textoLimpio = string.IsNullOrWhiteSpace(texto) ? string.Empty : texto.Trim();
            if (textoLimpio.Length == 0 || textoLimpio.Length > 100)
                throw new Exception("Mensaje: texto obligatorio (máx. 100 caracteres)");

            var categoriaLimpia = string.IsNullOrWhiteSpace(categoriaCod) ? string.Empty : categoriaCod.Trim().ToUpperInvariant();
            if (categoriaLimpia.Length != 3)
                throw new Exception("Mensaje: categoría de 3 caracteres");

            if (fechaCaducidad.Date <= DateTime.Today)
                throw new Exception("Mensaje: fecha de caducidad posterior al día de hoy");

            if (destinatarios == null)
                throw new Exception("Mensaje: destinatarios obligatorios");

            var listaDestinatarios = destinatarios.ToList();
            if (listaDestinatarios.Count == 0)
                throw new Exception("Mensaje: al menos un destinatario");

            var remitenteNombre = remitente.Username == null ? string.Empty : remitente.Username.Trim().ToUpperInvariant();
            var tieneRemitente = (from dest in listaDestinatarios
                                  let nombre = dest.Username == null ? string.Empty : dest.Username.Trim().ToUpperInvariant()
                                  where nombre == remitenteNombre
                                  select dest).Any();
            if (tieneRemitente)
                throw new Exception("Mensaje: remitente no puede ser destinatario");

            var duplicados = (from dest in listaDestinatarios
                              let nombre = dest.Username == null ? string.Empty : dest.Username.Trim().ToUpperInvariant()
                              group dest by nombre into grupo
                              where grupo.Key.Length > 0 && grupo.Count() > 1
                              select grupo.Key).Any();
            if (duplicados)
                throw new Exception("Mensaje: destinatarios repetidos");
        }

        public static void ValidarDestinatarios(IEnumerable<Usuario> destinatarios)
        {
            if (destinatarios == null || !destinatarios.Any())
                throw new Exception("Mensaje: al menos un destinatario");
        }

        private static int CalcularEdad(DateTime nacimiento, DateTime hoy)
        {
            var edad = hoy.Year - nacimiento.Year;
            if (nacimiento.Date > hoy.AddYears(-edad))
                edad--;

            return edad;
        }
    }
}
