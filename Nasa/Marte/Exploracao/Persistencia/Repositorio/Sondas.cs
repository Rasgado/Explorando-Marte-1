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
            try
            {
                return Todas().AsQueryable().Where(onde => onde.Id.Equals(id)).FirstOrDefault(); ;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Sonda ObterPorNome(string nome)
        {
            try
            {
                return Todas().AsQueryable().Where(onde => onde.Nome.Equals(nome)).FirstOrDefault(); ;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void Gravar(Sonda sonda)
        {
            if (!sonda.MeusDadosSaoValidos())
                try
                {
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
                catch (Exception)
                {
                    throw;
                }
        }

        private bool NovaSonda(Sonda sonda)
        {
            return sonda.Id.ToString().Equals("00000000-0000-0000-0000-000000000000");
        }

        public List<Sonda> ObterTodas()
        {
            try
            {
                return Todas().AsQueryable().ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        private IMongoCollection<Sonda> Todas()
        {
            return BancoDeDados.GetCollection<Sonda>("Sonda");
        }
    }
}
