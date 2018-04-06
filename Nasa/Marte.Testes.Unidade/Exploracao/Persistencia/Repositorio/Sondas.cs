using Marte.Exploracao.Dominio.Entidade;
using Marte.Exploracao.Dominio.ObjetoDeValor;
using Marte.Exploracao.Dominio.Repositorio;
using Marte.Exploracao.Dominio.Servico;
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
            var coordenada = new Coordenada(5, 5);
            var planalto = new Planalto();
            planalto.Criar(coordenada);

            var movimentoSempreParaFrente = new MovimentoParaFrente();
            var sonda = new Sonda("Mark I");

            sonda.Explorar(planalto);

            var posicaoDesejada = new Posicao(1, 2);
            sonda.IniciarEm(posicaoDesejada, DirecaoCardinal.Norte);

            sonda.Vire(Direcao.Esqueda);
            sonda.Move(movimentoSempreParaFrente);


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
