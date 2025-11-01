using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using ModeloEF.Entities;

namespace ModeloEF.Services
{
    /// <summary>
    /// Servicio de usuarios que encapsula el acceso a la base mediante Entity Framework.
    /// </summary>
    public class UsuariosServicio
    {
        /// <summary>
        /// Obtiene todos los usuarios ordenados alfabéticamente.
        /// </summary>
        public IList<Usuario> ObtenerTodos()
        {
            using (var contexto = new BiosMessengerContext())
            {
                return contexto.Usuarios
                    .OrderBy(u => u.Username)
                    .ToList();
            }
        }

        public int ContarUsuarios()
        {
            using (var contexto = new BiosMessengerContext())
            {
                return contexto.Usuarios.Count();
            }
        }

        /// <summary>
        /// Obtiene un usuario por identificador.
        /// </summary>
        public Usuario ObtenerPorUsername(string username)
        {
            using (var contexto = new BiosMessengerContext())
            {
                return contexto.Usuarios
                    .SingleOrDefault(u => u.Username == username);
            }
        }

        /// <summary>
        /// Da de alta un nuevo usuario aplicando las validaciones de negocio.
        /// </summary>
        public void Crear(Usuario usuario)
        {
            Validador.ValidarUsuario(usuario);

            using (var contexto = new BiosMessengerContext())
            {
                if (contexto.Usuarios.Any(u => u.Username == usuario.Username))
                {
                    throw new InvalidOperationException("Ya existe un usuario con el nombre especificado.");
                }

                contexto.Usuarios.Add(usuario);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        public void Actualizar(Usuario usuario)
        {
            Validador.ValidarActualizacionUsuario(usuario);

            using (var contexto = new BiosMessengerContext())
            {
                var existente = contexto.Usuarios.SingleOrDefault(u => u.Username == usuario.Username);
                if (existente == null)
                {
                    throw new InvalidOperationException("El usuario indicado no existe.");
                }

                contexto.Entry(existente).CurrentValues.SetValues(usuario);
                contexto.SaveChanges();
            }
        }

        /// <summary>
        /// Elimina un usuario utilizando el procedimiento almacenado provisto.
        /// Se realizan verificaciones previas para respetar las reglas de negocio.
        /// </summary>
        public void Eliminar(string username)
        {
            using (var contexto = new BiosMessengerContext())
            {
                var usuario = contexto.Usuarios
                    .Include("MensajesEnviados")
                    .Include("MensajesDestinados")
                    .SingleOrDefault(u => u.Username == username);

                if (usuario == null)
                {
                    throw new InvalidOperationException("El usuario indicado no existe.");
                }

                Validador.ValidarEliminacionUsuario(usuario);

                var parametros = new[]
                {
                    new SqlParameter("@Username", SqlDbType.Char, 8) {Value = username},
                    new SqlParameter
                    {
                        ParameterName = "@Ret",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Output
                    }
                };

                contexto.Database.ExecuteSqlCommand("EXEC spUsuario_Baja @Username, @Ret OUTPUT", parametros);

                var resultado = (int)parametros[1].Value;
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
            }
        }
    }
}
