using System.Collections.Generic;
using EntidadesCompartidas.Entidades;
using Persistencia.Contexto;
using Persistencia.Repositorios;

namespace Logica.Servicios
{
    public class CategoriasService
    {
        public IEnumerable<Categoria> Listar()
        {
            using (var contexto = new BiosMessengerContext())
            {
                var repo = new RepositorioCategorias(contexto);
                return repo.Listar();
            }
        }
    }
}
