using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using EntidadesCompartidas.Entidades;
using Persistencia.Contexto;

namespace Persistencia.Repositorios
{
    public class RepositorioMensajes
    {
        private readonly BiosMessengerContext _contexto;

        public RepositorioMensajes(BiosMessengerContext contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }

        public IQueryable<Mensaje> ObtenerConsultaCompleta()
        {
            return _contexto.Mensajes
                .Include(m => m.Remitente)
                .Include(m => m.Categoria)
                .Include(m => m.DestinatariosInternos.Select(md => md.Destinatario));
        }

        public Mensaje? ObtenerPorId(int id)
        {
            return ObtenerConsultaCompleta().FirstOrDefault(m => m.Id == id);
        }

        public Mensaje AltaMensaje(Mensaje mensaje, IEnumerable<string> destinatarios)
        {
            if (mensaje == null)
            {
                throw new ArgumentNullException(nameof(mensaje));
            }

            var listaDestinatarios = destinatarios?.Select(d => d.Trim()).Where(d => !string.IsNullOrWhiteSpace(d)).Distinct().ToList() ?? new List<string>();

            var parametros = new[]
            {
                new SqlParameter("@Asunto", mensaje.Asunto),
                new SqlParameter("@Texto", mensaje.Texto),
                new SqlParameter("@CategoriaCod", SqlDbType.Char, 3) { Value = mensaje.CategoriaCod.Trim() },
                new SqlParameter("@Remitente", SqlDbType.Char, 8) { Value = mensaje.RemitenteUsername.Trim() },
                new SqlParameter("@FechaCaducidad", mensaje.FechaCaducidad),
                new SqlParameter
                {
                    ParameterName = "@Id",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                }
            };

            _contexto.Database.ExecuteSqlCommand(
                "EXEC spMensaje_Alta @Asunto, @Texto, @CategoriaCod, @Remitente, @FechaCaducidad, @Id OUTPUT",
                parametros);

            mensaje.Id = (int)(parametros[5].Value ?? 0);

            foreach (var destino in listaDestinatarios)
            {
                var paramId = new SqlParameter("@IdMsg", SqlDbType.Int) { Value = mensaje.Id };
                var paramDestino = new SqlParameter("@Destino", SqlDbType.Char, 8) { Value = destino };
                _contexto.Database.ExecuteSqlCommand("EXEC spMensaje_AddDestinatario @IdMsg, @Destino", paramId, paramDestino);
            }

            return ObtenerPorId(mensaje.Id) ?? mensaje;
        }
    }
}
