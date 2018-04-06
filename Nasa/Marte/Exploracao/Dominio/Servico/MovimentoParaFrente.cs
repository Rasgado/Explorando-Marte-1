using Marte.Exploracao.Dominio.Contratos;
using Marte.Exploracao.Dominio.ObjetoDeValor;

namespace Marte.Exploracao.Dominio.Servico
{
    public class MovimentoParaFrente : IMovimento
    {
        public Posicao Executar(DirecaoCardinal direcaoCardinalAtual, Posicao posicaoAtual)
        {
            Posicao novaPosicao = null;
            switch (direcaoCardinalAtual)
            {
                case DirecaoCardinal.Norte:
                    novaPosicao = new Posicao(posicaoAtual.X, posicaoAtual.Y + 1);
                    break;
                case DirecaoCardinal.Leste:
                    novaPosicao = new Posicao(posicaoAtual.X + 1, posicaoAtual.Y);
                    break;
                case DirecaoCardinal.Sul:
                    novaPosicao = new Posicao(posicaoAtual.X, posicaoAtual.Y - 1);
                    break;
                case DirecaoCardinal.Oeste:
                    novaPosicao = new Posicao(posicaoAtual.X - 1, posicaoAtual.Y);
                    break;
            }
            return novaPosicao;
        }
    }
}

