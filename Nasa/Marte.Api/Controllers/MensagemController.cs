using Marte.CamadaAnticorrupcao;
using Marte.Exploracao.Persistencia.BancoDeDados;
using Marte.Exploracao.Persistencia.Contratos;
using Marte.PreocupacoesTransversal.Exploracao.Persistencia.BancoDeDados;
using MongoDB.Driver;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Marte.Api.Controllers
{
    public class MensagemController : BaseController
    {
        private ExploradorDePlanalto explorador;
        private IConexaoComOBanco conexaoComOBanco;
        private IMongoDatabase db;

        public MensagemController()
        {
            conexaoComOBanco = new ConexaoComOBanco();
            db = new ProvedorDeAcesso().Criar(conexaoComOBanco);
            explorador = new ExploradorDePlanalto(db, conexaoComOBanco);
        }

        [HttpPost]
        [Route("api/Mensagem")]
        public Task<HttpResponseMessage> Post([FromBody]dynamic body)
        {
            var conteudo = (string)body.conteudo;

            var retorno = explorador.Iniciar(conteudo);

            QuebraDeEspeficacao = explorador.RegrasNegocio;

            return CreateResponse(System.Net.HttpStatusCode.Created, retorno);
        }
    }
}
