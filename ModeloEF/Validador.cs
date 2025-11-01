using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ModeloEF.Entities;

namespace ModeloEF
{
    /// <summary>
    /// Contiene todas las validaciones.
    /// </summary>
    public static class Validador
    {
        private static readonly Regex UsuarioRegex = new Regex("^[A-Z0-9]{8}$", RegexOptions.Compiled);
        private static readonly Regex PasswordRegex = new Regex("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)[A-Za-z\\d]{8}$", RegexOptions.Compiled);
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public static void ValidarUsuario(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario));
            }

            if (!UsuarioRegex.IsMatch(usuario.Username ?? string.Empty))
            {
                throw new ArgumentException("El nombre de usuario debe contener exactamente ocho caracteres alfanuméricos en mayúscula.");
            }

            if (!PasswordRegex.IsMatch(usuario.Pass ?? string.Empty))
            {
                throw new ArgumentException("La contraseña debe tener ocho caracteres, incluir mayúsculas, minúsculas y dígitos.");
            }

            if (string.IsNullOrWhiteSpace(usuario.NombreCompleto) || usuario.NombreCompleto.Trim().Length > 50)
            {
                throw new ArgumentException("El nombre completo es obligatorio y no puede superar los 50 caracteres.");
            }

            if (!EmailRegex.IsMatch(usuario.Email ?? string.Empty))
            {
                throw new ArgumentException("El correo electrónico no tiene un formato válido.");
            }

            if (usuario.FechaNacimiento.Date >= DateTime.Today)
            {
                throw new ArgumentException("La fecha de nacimiento debe estar en el pasado.");
            }

            var edad = CalcularEdad(usuario.FechaNacimiento, DateTime.Today);
            if (edad < 18)
            {
                throw new ArgumentException("Los usuarios deben ser mayores de edad.");
            }
        }

        public static void ValidarActualizacionUsuario(Usuario usuario)
        {
            ValidarUsuario(usuario);
        }

        public static void ValidarEliminacionUsuario(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario));
            }

            if (usuario.MensajesEnviados.Any())
            {
                throw new InvalidOperationException("No es posible eliminar un usuario con mensajes enviados.");
            }

            if (usuario.MensajesDestinados.Any())
            {
                throw new InvalidOperationException("No es posible eliminar un usuario que haya recibido mensajes.");
            }
        }

        public static void ValidarMensaje(string asunto, string texto, string categoriaCod, Usuario remitente, DateTime fechaCaducidad, IEnumerable<Usuario> destinatarios)
        {
            if (remitente == null)
            {
                throw new ArgumentNullException(nameof(remitente));
            }

            if (string.IsNullOrWhiteSpace(asunto) || asunto.Trim().Length > 50)
            {
                throw new ArgumentException("El asunto es obligatorio y debe tener hasta 50 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(texto) || texto.Trim().Length > 100)
            {
                throw new ArgumentException("El texto es obligatorio y debe tener hasta 100 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(categoriaCod) || categoriaCod.Length != 3)
            {
                throw new ArgumentException("La categoría es obligatoria y debe corresponder a un código de tres caracteres.");
            }

            if (fechaCaducidad.Date <= DateTime.Today)
            {
                throw new ArgumentException("La fecha de caducidad debe ser al menos el día siguiente al envío.");
            }

            if (destinatarios == null || !destinatarios.Any())
            {
                throw new ArgumentException("Debe seleccionarse al menos un destinatario.");
            }

            if (destinatarios.Any(d => d.Username == remitente.Username))
            {
                throw new ArgumentException("El remitente no puede figurar como destinatario.");
            }

            if (destinatarios.Select(d => d.Username).Distinct().Count() != destinatarios.Count())
            {
                throw new ArgumentException("Los destinatarios no pueden repetirse.");
            }
        }

        public static void ValidarDestinatarios(IEnumerable<Usuario> destinatarios)
        {
            if (destinatarios == null || !destinatarios.Any())
            {
                throw new ArgumentException("El mensaje debe tener al menos un destinatario.");
            }
        }

        private static int CalcularEdad(DateTime nacimiento, DateTime hoy)
        {
            var edad = hoy.Year - nacimiento.Year;
            if (nacimiento.Date > hoy.AddYears(-edad))
            {
                edad--;
            }

            return edad;
        }
    }
}
