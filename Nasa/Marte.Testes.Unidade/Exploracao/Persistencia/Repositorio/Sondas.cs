using Marte.Exploracao.Dominio.Entidade;
using Marte.Exploracao.Dominio.Repositorio;
using System;
using System.Collections.Generic;

namespace Marte.Testes.Unidade.Exploracao.Persistencia.Repositorio
{
    public class Sondas : ISondas
    {
        public void Gravar(Sonda sonda)
        {
        }

        public Sonda ObterPorId(Guid id)
        {
            var sonda = new Sonda("Mark I");

            return sonda;
        }

        public Sonda ObterPorNome(string nome)
        {
            return ObterPorId(Guid.NewGuid());
        }

        public List<Sonda> ObterTodas()
        {
            return new List<Sonda>() { ObterPorId(Guid.NewGuid()) };
        }
    }
}
