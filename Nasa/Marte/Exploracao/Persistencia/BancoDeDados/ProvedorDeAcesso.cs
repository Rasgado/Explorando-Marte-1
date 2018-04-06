using Marte.Exploracao.Persistencia.Contratos;
using MongoDB.Driver;

namespace Marte.Exploracao.Persistencia.BancoDeDados
{
    public class ProvedorDeAcesso
    {
        public IMongoDatabase Criar(IConexaoComOBanco conexaoComOBanco)
        {
            IMongoClient client = new MongoClient(conexaoComOBanco.Obter());
            IMongoDatabase database = client.GetDatabase("Marte");

            return database;
        }
    }
}
