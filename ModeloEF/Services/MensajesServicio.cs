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
    /// Servicio de mensajería que encapsula la lógica de acceso a datos y reglas de negocio.
    /// </summary>
    public class MensajesServicio
    {
        public IList<Categoria> ObtenerCategorias()
        {
            using (var contexto = new BiosMessengerContext())
            {
                return contexto.Categorias
                    .OrderBy(c => c.Nombre)
                    .ToList();
            }
        }

        public int ContarMensajes()
        {
            using (var contexto = new BiosMessengerContext())
            {
                return contexto.Mensajes.Count();
            }
        }

        public IList<Mensaje> ObtenerBandejaSalida(string remitente)
        {
            using (var contexto = new BiosMessengerContext())
            {
                return contexto.Mensajes
                    .Include("Destinatarios")
                    .Include("Remitente")
                    .Include("Categoria")
                    .Where(m => m.RemitenteUsername == remitente)
                    .OrderByDescending(m => m.FechaEnvio)
                    .ToList();
            }
        }

        public IList<Mensaje> ObtenerBandejaEntrada(string destinatario)
        {
            using (var contexto = new BiosMessengerContext())
            {
                var hoy = DateTime.Now;
                return contexto.Mensajes
                    .Include("Destinatarios")
                    .Include("Remitente")
                    .Include("Categoria")
                    .Where(m => m.Destinatarios.Any(u => u.Username == destinatario) && m.FechaCaducidad > hoy)
                    .OrderByDescending(m => m.FechaEnvio)
                    .ToList();
            }
        }

        /// <summary>
        /// Crea un nuevo mensaje utilizando los procedimientos almacenados provistos.
        /// </summary>
        public void CrearMensaje(string asunto, string texto, string categoriaCod, string remitenteUsername, DateTime fechaCaducidad, IEnumerable<string> destinatarios)
        {
            using (var contexto = new BiosMessengerContext())
            {
                remitenteUsername = remitenteUsername.ToUpperInvariant();
                var remitente = contexto.Usuarios.SingleOrDefault(u => u.Username == remitenteUsername);
                if (remitente == null)
                {
                    throw new InvalidOperationException("El remitente indicado no existe.");
                }

                var categoriaNormalizada = categoriaCod.ToUpperInvariant();

                var destinatarioEntidades = contexto.Usuarios
                    .Where(u => destinatarios.Contains(u.Username))
                    .ToList();

                if (destinatarioEntidades.Count != destinatarios.Count())
                {
                    throw new InvalidOperationException("Existen destinatarios seleccionados que no se encuentran en el sistema.");
                }

                Validador.ValidarMensaje(asunto, texto, categoriaNormalizada, remitente, fechaCaducidad, destinatarioEntidades);

                var parametrosAlta = new[]
                {
                    new SqlParameter("@Asunto", SqlDbType.VarChar, 50) {Value = asunto},
                    new SqlParameter("@Texto", SqlDbType.VarChar, 100) {Value = texto},
                    new SqlParameter("@CategoriaCod", SqlDbType.Char, 3) {Value = categoriaNormalizada},
                    new SqlParameter("@Remitente", SqlDbType.Char, 8) {Value = remitenteUsername},
                    new SqlParameter("@FechaCaducidad", SqlDbType.DateTime) {Value = fechaCaducidad},
                    new SqlParameter
                    {
                        ParameterName = "@Id",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Output
                    }
                };

                contexto.Database.ExecuteSqlCommand("EXEC spMensaje_Alta @Asunto, @Texto, @CategoriaCod, @Remitente, @FechaCaducidad, @Id OUTPUT", parametrosAlta);
                var idMensaje = (int)parametrosAlta.Last().Value;

                foreach (var destinatario in destinatarioEntidades)
                {
                    var parametrosDestinatario = new[]
                    {
                        new SqlParameter("@IdMsg", SqlDbType.Int) {Value = idMensaje},
                        new SqlParameter("@Destino", SqlDbType.Char, 8) {Value = destinatario.Username}
                    };

                    contexto.Database.ExecuteSqlCommand("EXEC spMensaje_AddDestinatario @IdMsg, @Destino", parametrosDestinatario);
                }
            }
        }

        /// <summary>
        /// Obtiene la cantidad de mensajes con más de cinco destinatarios empleando una subconsulta LINQ.
        /// </summary>
        public int ContarMensajesConMasDeCincoDestinatarios()
        {
            using (var contexto = new BiosMessengerContext())
            {
                var query = from mensaje in contexto.Mensajes
                            where (from destino in contexto.Usuarios
                                   where destino.MensajesDestinados.Any(m => m.Id == mensaje.Id)
                                   select destino.Username).Count() > 5
                            select mensaje.Id;

                return query.Count();
            }
        }
    }
}
