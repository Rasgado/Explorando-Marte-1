using Marte.Exploracao.Dominio.ObjetoDeValor;

namespace Marte.Exploracao.Dominio.Contratos
{
    public interface IMovimento
    {
        Posicao Executar(DirecaoCardinal direcaoAtual, Posicao posicaoAtual);
    }

}
