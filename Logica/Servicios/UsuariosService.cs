using System;
using System.Collections.Generic;
using EntidadesCompartidas;
using EntidadesCompartidas.Entidades;
using Persistencia.Contexto;
using Persistencia.Repositorios;

namespace Logica.Servicios
{
    public class UsuariosService
    {
        public IEnumerable<Usuario> Listar()
        {
            using (var contexto = new BiosMessengerContext())
            {
                var repo = new RepositorioUsuarios(contexto);
                return repo.Listar();
            }
        }

        public Usuario? Buscar(string username)
        {
            using (var contexto = new BiosMessengerContext())
            {
                var repo = new RepositorioUsuarios(contexto);
                return repo.BuscarPorUsername(username);
            }
        }

        public Usuario Crear(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario));
            }

            usuario.ValidarAlta();

            using (var contexto = new BiosMessengerContext())
            {
                var repo = new RepositorioUsuarios(contexto);
                if (repo.BuscarPorUsername(usuario.Username) != null)
                {
                    throw new InvalidOperationException("Ya existe un usuario con ese nombre.");
                }

                repo.Alta(usuario);
                return repo.BuscarPorUsername(usuario.Username) ?? usuario;
            }
        }

        public void ActualizarPassword(string username, string nuevoPassword)
        {
            Validador.ValidarPassword(nuevoPassword);

            using (var contexto = new BiosMessengerContext())
            {
                var repo = new RepositorioUsuarios(contexto);
                repo.CambiarPassword(username, nuevoPassword);
            }
        }

        public void ModificarPerfil(Usuario usuario)
        {
            if (usuario == null)
            {
                throw new ArgumentNullException(nameof(usuario));
            }

            usuario.ValidarModificacion();

            using (var contexto = new BiosMessengerContext())
            {
                var repo = new RepositorioUsuarios(contexto);
                repo.Modificar(usuario);
            }
        }

        public void Eliminar(string username)
        {
            using (var contexto = new BiosMessengerContext())
            {
                var repo = new RepositorioUsuarios(contexto);
                repo.Baja(username);
            }
        }
    }
}
