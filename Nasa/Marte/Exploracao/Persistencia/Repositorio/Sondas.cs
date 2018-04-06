using Marte.Exploracao.Dominio.Entidade;
using Marte.Exploracao.Dominio.Repositorio;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Marte.Exploracao.Persistencia.Repositorio
{
    public class Sondas : ISondas
    {
        private readonly IMongoDatabase BancoDeDados;

        public Sondas(IMongoDatabase bancoDeDados)
        {
            BancoDeDados = bancoDeDados ?? throw new ArgumentException("A conexão com o banco de dados não foi informada.");
        }

        public Sonda ObterPorId(Guid id)
        {
            return Todas().AsQueryable().Where(onde => onde.Id.Equals(id)).FirstOrDefault(); ;
        }

        public Sonda ObterPorNome(string nome)
        {
            return Todas().AsQueryable().Where(onde => onde.Nome.Equals(nome)).FirstOrDefault(); ;
        }
        public void Gravar(Sonda sonda)
        {
            if (!sonda.MeusDadosSaoValidos())
                if (NovaSonda(sonda))
                {
                    Todas().InsertOne(sonda);
                }
                else
                {
                    Expression<Func<Sonda, bool>> filter = x => x.Id.Equals(sonda.Id);
                    Todas().ReplaceOne(filter, sonda);
                }
        }

        private static bool NovaSonda(Sonda sonda)
        {
            return sonda.Id.ToString().Equals("00000000-0000-0000-0000-000000000000");
        }

        private IMongoCollection<Sonda> Todas()
        {
            return BancoDeDados.GetCollection<Sonda>("Sonda");
        }

        public List<Sonda> ObterTodas()
        {
            return Todas().AsQueryable().ToList();
        }
    }
}
