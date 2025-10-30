using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using EntidadesCompartidas.Entidades;
using Persistencia.Contexto;

namespace Persistencia.Repositorios
{
    public class RepositorioUsuarios
    {
        private readonly BiosMessengerContext _contexto;

        public RepositorioUsuarios(BiosMessengerContext contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }

        public IEnumerable<Usuario> Listar()
        {
            return _contexto.Usuarios
                .OrderBy(u => u.Username)
                .ToList();
        }

        public Usuario? BuscarPorUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return null;
            }

            var normalizado = username.Trim();
            return _contexto.Usuarios
                .Include(u => u.MensajesEnviados)
                .Include(u => u.MensajesRecibidos.Select(md => md.Mensaje))
                .FirstOrDefault(u => u.Username.Trim() == normalizado);
        }

        public void Alta(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario));
            }

            _contexto.Usuarios.Add(usuario);
            _contexto.SaveChanges();
        }

        public void Modificar(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario));
            }

            _contexto.Entry(usuario).State = EntityState.Modified;
            _contexto.SaveChanges();
        }

        public void CambiarPassword(string username, string nuevoPassword)
        {
            var usuario = BuscarPorUsername(username);
            if (usuario == null)
            {
                throw new InvalidOperationException("El usuario no existe.");
            }

            usuario.Pass = nuevoPassword;
            _contexto.SaveChanges();
        }

        public void Baja(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentException("Debe indicar un usuario.", nameof(username));
            }

            var parametro = new SqlParameter("@Username", System.Data.SqlDbType.Char, 8)
            {
                Value = username.Trim()
            };

            try
            {
                _contexto.Database.ExecuteSqlCommand("EXEC spUsuario_Baja @Username", parametro);
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("No se puede eliminar el usuario porque tiene información relacionada.", ex);
            }
        }
    }
}
