using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EntidadesCompartidas.Entidades;

namespace EntidadesCompartidas
{
    public static class Validador
    {
        private static readonly Regex EmailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

        public static void ValidarUsuarioAlta(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario));
            }

            ValidarUsername(usuario.Username);
            ValidarPassword(usuario.Pass);
            ValidarNombre(usuario.NombreCompleto);
            ValidarEmail(usuario.Email);
            ValidarFecha(usuario.FechaNacimiento);
        }

        public static void ValidarUsuarioModificar(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario));
            }

            ValidarNombre(usuario.NombreCompleto);
            ValidarEmail(usuario.Email);
            ValidarFecha(usuario.FechaNacimiento);
        }

        public static void ValidarPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length != 8)
            {
                throw new ArgumentException("La contraseña debe contener exactamente 8 caracteres.");
            }

            var letras = password.Count(char.IsLetter);
            var numeros = password.Count(char.IsDigit);
            var simbolos = password.Length - letras - numeros;

            if (letras != 3 || numeros != 3 || simbolos != 2)
            {
                throw new ArgumentException("La contraseña debe tener 3 letras, 3 dígitos y 2 símbolos.");
            }
        }

        public static void ValidarCategoria(Categoria categoria)
        {
            if (categoria == null)
            {
                throw new ArgumentNullException(nameof(categoria));
            }

            if (string.IsNullOrWhiteSpace(categoria.Codigo) || categoria.Codigo.Trim().Length != 3)
            {
                throw new ArgumentException("El código de categoría debe tener 3 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(categoria.Nombre))
            {
                throw new ArgumentException("El nombre de la categoría es obligatorio.");
            }
        }

        public static void ValidarMensajeAlta(Mensaje mensaje, IEnumerable<string> destinatarios)
        {
            if (mensaje == null)
            {
                throw new ArgumentNullException(nameof(mensaje));
            }

            if (string.IsNullOrWhiteSpace(mensaje.Asunto))
            {
                throw new ArgumentException("El asunto es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(mensaje.Texto))
            {
                throw new ArgumentException("El texto es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(mensaje.RemitenteUsername))
            {
                throw new ArgumentException("El remitente es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(mensaje.CategoriaCod))
            {
                throw new ArgumentException("La categoría es obligatoria.");
            }

            if ((mensaje.FechaCaducidad.Date - mensaje.FechaEnvio.Date).TotalDays < 1)
            {
                throw new ArgumentException("La fecha de caducidad debe ser al menos un día posterior al envío.");
            }

            if (destinatarios == null || !destinatarios.Any())
            {
                throw new ArgumentException("Debe indicar al menos un destinatario.");
            }

            foreach (var dest in destinatarios)
            {
                ValidarUsername(dest);
            }
        }

        private static void ValidarUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Trim().Length != 8)
            {
                throw new ArgumentException("El nombre de usuario debe tener exactamente 8 caracteres.");
            }

            if (username.Contains(" "))
            {
                throw new ArgumentException("El nombre de usuario no puede contener espacios.");
            }
        }

        private static void ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException("El nombre completo es obligatorio.");
            }
        }

        private static void ValidarEmail(string email)
        {
            if (!EmailRegex.IsMatch(email ?? string.Empty))
            {
                throw new ArgumentException("Formato de correo inválido.");
            }
        }

        private static void ValidarFecha(DateTime fecha)
        {
            if (fecha.Date > DateTime.Today)
            {
                throw new ArgumentException("La fecha de nacimiento debe ser una fecha pasada.");
            }
        }
    }
}
