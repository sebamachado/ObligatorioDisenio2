using System;
using System.Collections.Generic;
using System.Linq;
using EntidadesCompartidas;
using EntidadesCompartidas.Entidades;
using Logica.Dto;
using Persistencia.Contexto;
using Persistencia.Repositorios;

namespace Logica.Servicios
{
    public class MensajesService
    {
        public Mensaje Enviar(string remitenteUsername, string categoriaCod, string asunto, string texto, DateTime fechaCaducidad, IEnumerable<string> destinatarios)
        {
            var mensaje = new Mensaje
            {
                RemitenteUsername = remitenteUsername?.Trim() ?? string.Empty,
                CategoriaCod = categoriaCod?.Trim() ?? string.Empty,
                Asunto = asunto ?? string.Empty,
                Texto = texto ?? string.Empty,
                FechaEnvio = DateTime.Now,
                FechaCaducidad = fechaCaducidad
            };

            var destinatariosList = destinatarios?.ToList() ?? new List<string>();
            mensaje.ValidarAlta(destinatariosList);

            using (var contexto = new BiosMessengerContext())
            {
                var repo = new RepositorioMensajes(contexto);
                return repo.AltaMensaje(mensaje, destinatariosList);
            }
        }

        public IEnumerable<Mensaje> ObtenerBandeja(string username)
        {
            var normalizado = username?.Trim() ?? string.Empty;
            var ahora = DateTime.Now;

            using (var contexto = new BiosMessengerContext())
            {
                var repo = new RepositorioMensajes(contexto);
                return repo.ObtenerConsultaCompleta()
                    .Where(m => m.RemitenteUsername.Trim() == normalizado
                                 || (m.DestinatariosInternos.Any(md => md.DestinoUsername.Trim() == normalizado)
                                     && m.FechaCaducidad > ahora))
                    .OrderByDescending(m => m.FechaEnvio)
                    .ToList();
            }
        }

        public IEnumerable<Mensaje> ObtenerEnviados(string username)
        {
            var normalizado = username?.Trim() ?? string.Empty;

            using (var contexto = new BiosMessengerContext())
            {
                var repo = new RepositorioMensajes(contexto);
                return repo.ObtenerConsultaCompleta()
                    .Where(m => m.RemitenteUsername.Trim() == normalizado)
                    .OrderByDescending(m => m.FechaEnvio)
                    .ToList();
            }
        }

        public TotalesDto ObtenerTotales()
        {
            using (var contexto = new BiosMessengerContext())
            {
                var ahora = DateTime.Now;

                var totalUsuarios = contexto.Usuarios.Count();
                var mensajes = contexto.Mensajes;

                var activos = mensajes.Count(m => m.FechaCaducidad > ahora);
                var expirados = mensajes.Count(m => m.FechaCaducidad <= ahora);

                var totalMasDeCinco = (from m in contexto.Mensajes
                                       where (from md in contexto.MensajeDestinatarios
                                              where md.MensajeId == m.Id
                                              select md).Count() > 5
                                       select m.Id).Count();

                return new TotalesDto
                {
                    TotalUsuarios = totalUsuarios,
                    MensajesActivos = activos,
                    MensajesExpirados = expirados,
                    MensajesConMasDeCincoDestinatarios = totalMasDeCinco
                };
            }
        }
    }
}
