using Marte.Exploracao.Dominio.Entidade;
using System;
using System.Collections.Generic;

namespace Marte.Exploracao.Dominio.Repositorio
{
    public interface ISondas
    {
        Sonda ObterPorId(Guid id);
        Sonda ObterPorNome(string nome);
        List<Sonda> ObterTodas();
        void Gravar(Sonda sonda);
    }
}
