using System;
using System.Collections.Generic;
using System.Linq;
using EntidadesCompartidas.Entidades;
using Persistencia.Contexto;

namespace Persistencia.Repositorios
{
    public class RepositorioCategorias
    {
        private readonly BiosMessengerContext _contexto;

        public RepositorioCategorias(BiosMessengerContext contexto)
        {
            _contexto = contexto ?? throw new ArgumentNullException(nameof(contexto));
        }

        public IEnumerable<Categoria> Listar()
        {
            return _contexto.Categorias
                .OrderBy(c => c.Codigo)
                .ToList();
        }

        public Categoria? ObtenerPorCodigo(string codigo)
        {
            return string.IsNullOrWhiteSpace(codigo)
                ? null
                : _contexto.Categorias.FirstOrDefault(c => c.Codigo.Trim() == codigo.Trim());
        }
    }
}
