using Marte.Exploracao.Dominio.Contratos;
using Marte.Exploracao.Dominio.Entidade;
using Marte.Exploracao.Dominio.ObjetoDeValor;
using Marte.Exploracao.Dominio.Servico;
using Marte.Exploracao.Persistencia.BancoDeDados;
using Marte.Exploracao.Persistencia.Contratos;
using Marte.Exploracao.Persistencia.Repositorio;
using Marte.PreocupacoesTransversal.Exploracao.Persistencia.BancoDeDados;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;

namespace Marte.Testes.Integracao.Exploracao.Entidade
{
    [TestClass]
    public class SondaTeste
    {
        private Planalto planalto;
        private Sonda sonda;
        private IMovimento movimentoSempreParaFrente;
        private IConexaoComOBanco conexaoComOBanco;

        [TestInitialize]
        public void Iniciar()
        {
            var coordenada = new Coordenada(5, 5);
            planalto = new Planalto();
            planalto.Criar(coordenada);

            movimentoSempreParaFrente = new MovimentoParaFrente();

            conexaoComOBanco = new ConexaoComOBanco();
        }

        [TestMethod]
        public void Deve_fazer_a_exploracao_com_a_sonda_iniciando_em_12N_com_a_serie_de_instruncoes_LMLMLMLMM()
        {
            IMongoDatabase db = new ProvedorDeAcesso().Criar(conexaoComOBanco);
            Sondas sondas = new Sondas(db);

            sonda = new Sonda("Mark I");
            sonda.Explorar(planalto);

            var posicaoDesejada = new Posicao(1, 2);
            var posicaoEsperada = new Posicao(1, 3);

            sonda.IniciarEm(posicaoDesejada, DirecaoCardinal.Norte);

            sonda.Vire(Direcao.Esqueda);
            sonda.Move(movimentoSempreParaFrente);
            sonda.Vire(Direcao.Esqueda);
            sonda.Move(movimentoSempreParaFrente);

            sondas.Gravar(sonda);
            var idGerado = sonda.Id;
            sonda = sondas.ObterPorId(idGerado);

            sonda.Vire(Direcao.Esqueda);
            sonda.Move(movimentoSempreParaFrente);
            sonda.Vire(Direcao.Esqueda);
            sonda.Move(movimentoSempreParaFrente);
            sonda.Move(movimentoSempreParaFrente);

            sondas.Gravar(sonda);
            var idEsperado = sonda.Id;

            Assert.AreNotEqual("00000000-0000-0000-0000-000000000000", sonda.Id.ToString());
            Assert.AreEqual(idEsperado, idGerado);
            Assert.AreEqual(posicaoEsperada, sonda.PosicaoAtual);
            Assert.AreEqual(DirecaoCardinal.Norte, sonda.DirecaoAtual);
        }

        [TestMethod]
        public void Nao_deve_gravar_uma_sonda_com_dados_invalidos()
        {
            IMongoDatabase db = new ProvedorDeAcesso().Criar(conexaoComOBanco);
            Sondas sondas = new Sondas(db);

            sonda = new Sonda("Mark Ierro");

            sondas.Gravar(sonda);

            Assert.IsTrue(sonda.QuebraDeEspeficacao.HouveViolacao());
        }
    }
}
