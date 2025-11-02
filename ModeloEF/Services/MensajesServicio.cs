using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Data.Entity;
using Categoria = ModeloEF.Categorias;
using Mensaje = ModeloEF.Mensajes;
using Usuario = ModeloEF.Usuarios;

namespace ModeloEF.Services
{
    /// <summary>
    /// Servicio de mensajería que encapsula la lógica de acceso a datos y reglas de negocio.
    /// </summary>
    public class MensajesServicio
    {
        public IList<Categoria> ObtenerCategorias()
        {
            using (var contexto = new BiosMessengerEntities1())
            {
                return contexto.Categorias
                    .OrderBy(c => c.Nombre)
                    .ToList();
            }
        }

        public int ContarMensajes()
        {
            using (var contexto = new BiosMessengerEntities1())
            {
                return contexto.Mensajes.Count();
            }
        }

        public IList<Mensaje> ObtenerBandejaSalida(string remitente)
        {
            using (var contexto = new BiosMessengerEntities1())
            {
                return contexto.Mensajes
                    .Include(m => m.Destinatarios)
                    .Include(m => m.Remitente)
                    .Include(m => m.Categoria)
                    .Where(m => m.Remitente.Username == remitente)
                    .OrderByDescending(m => m.FechaEnvio)
                    .ToList();
            }
        }

        public IList<Mensaje> ObtenerBandejaEntrada(string destinatario)
        {
            using (var contexto = new BiosMessengerEntities1())
            {
                var hoy = DateTime.Now;
                return contexto.Mensajes
                    .Include(m => m.Destinatarios)
                    .Include(m => m.Remitente)
                    .Include(m => m.Categoria)
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
            using (var contexto = new BiosMessengerEntities1())
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

                var idParametro = new SqlParameter
                {
                    ParameterName = "@Id",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                var retParametro = new SqlParameter
                {
                    ParameterName = "@Ret",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                var parametrosAlta = new[]
                {
                    new SqlParameter("@Asunto", SqlDbType.VarChar, 50) {Value = asunto},
                    new SqlParameter("@Texto", SqlDbType.VarChar, 100) {Value = texto},
                    new SqlParameter("@CategoriaCod", SqlDbType.Char, 3) {Value = categoriaNormalizada},
                    new SqlParameter("@Remitente", SqlDbType.Char, 8) {Value = remitenteUsername},
                    new SqlParameter("@FechaCaducidad", SqlDbType.DateTime) {Value = fechaCaducidad},
                    idParametro,
                    retParametro
                };

                contexto.Database.ExecuteSqlCommand("EXEC spMensaje_Alta @Asunto, @Texto, @CategoriaCod, @Remitente, @FechaCaducidad, @Id OUTPUT, @Ret OUTPUT", parametrosAlta);

                var resultadoAlta = (int)retParametro.Value;
                if (resultadoAlta < 0)
                {
                    LanzarExcepcionPorResultadoAlta(resultadoAlta);
                }

                var idMensaje = (int)idParametro.Value;

                foreach (var destinatario in destinatarioEntidades)
                {
                    var retParametroDestinatario = new SqlParameter
                    {
                        ParameterName = "@Ret",
                        SqlDbType = SqlDbType.Int,
                        Direction = ParameterDirection.Output
                    };

                    var parametrosDestinatario = new[]
                    {
                        new SqlParameter("@IdMsg", SqlDbType.Int) {Value = idMensaje},
                        new SqlParameter("@Destino", SqlDbType.Char, 8) {Value = destinatario.Username},
                        retParametroDestinatario
                    };

                    contexto.Database.ExecuteSqlCommand("EXEC spMensaje_AddDestinatario @IdMsg, @Destino, @Ret OUTPUT", parametrosDestinatario);

                    var resultadoDestinatario = (int)retParametroDestinatario.Value;
                    if (resultadoDestinatario < 0)
                    {
                        LanzarExcepcionPorResultadoDestinatario(resultadoDestinatario);
                    }
                }
            }
        }

        /// <summary>
        /// Obtiene la cantidad de mensajes con más de cinco destinatarios empleando una subconsulta LINQ.
        /// </summary>
        public int ContarMensajesConMasDeCincoDestinatarios()
        {
            using (var contexto = new BiosMessengerEntities1())
            {
                return contexto.Mensajes.Count(m => m.Destinatarios.Count() > 5);
            }
        }

        private static void LanzarExcepcionPorResultadoAlta(int codigo)
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

        private static void LanzarExcepcionPorResultadoDestinatario(int codigo)
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

}

