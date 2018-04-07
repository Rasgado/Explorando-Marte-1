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
                    return new Posicao(posicaoAtual.X, posicaoAtual.Y + 1);
                case DirecaoCardinal.Leste:
                    return new Posicao(posicaoAtual.X + 1, posicaoAtual.Y);
                case DirecaoCardinal.Sul:
                    return new Posicao(posicaoAtual.X, posicaoAtual.Y - 1);
                case DirecaoCardinal.Oeste:
                    return new Posicao(posicaoAtual.X - 1, posicaoAtual.Y);
            }
            return novaPosicao;
        }
    }
}

