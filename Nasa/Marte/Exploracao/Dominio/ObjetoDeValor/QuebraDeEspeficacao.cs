using Marte.Exploracao.Dominio.Contratos;
using System.Collections.Generic;
using System.Linq;

namespace Marte.Exploracao.Dominio.ObjetoDeValor
{
    public class QuebraDeEspeficacao : IEspecificacaoDeNegocio
    {
        private readonly IList<RegraDeNegocio> _regrasDeNegocio;

        public IEnumerable<RegraDeNegocio> RegrasDeNegocio { get { return _regrasDeNegocio; } }

        public QuebraDeEspeficacao()
        {
            _regrasDeNegocio = new List<RegraDeNegocio>();
        }

        public bool Contem(RegraDeNegocio regraDeNegocio)
        {
            return _regrasDeNegocio.Contains(regraDeNegocio);
        }

        public bool HouveViolacao()
        {
            return _regrasDeNegocio.ToList().Count != 0;
        }

        public void Adicionar(RegraDeNegocio regraDeNegocio)
        {
            if (regraDeNegocio != null & !Contem(regraDeNegocio))
                _regrasDeNegocio.Add(regraDeNegocio);
        }

        public override bool Equals(object obj)
        {
            var negocio = obj as QuebraDeEspeficacao;
            return negocio != null &&
                   EqualityComparer<IList<RegraDeNegocio>>.Default.Equals(_regrasDeNegocio, negocio._regrasDeNegocio);
        }

        public override int GetHashCode()
        {
            return -539705838 + EqualityComparer<IList<RegraDeNegocio>>.Default.GetHashCode(_regrasDeNegocio);
        }
    }
}

