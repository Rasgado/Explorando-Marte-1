using Marte.Exploracao.Dominio.Contratos;
using Marte.Exploracao.Dominio.Entidade;
using Marte.Exploracao.Dominio.ObjetoDeValor;
using Marte.Exploracao.Dominio.Servico;
using Marte.Exploracao.Persistencia.Contratos;
using Marte.Exploracao.Persistencia.Repositorio;
using MongoDB.Driver;
using System;


namespace Marte.CamadaAnticorrupcao
{
    public class ExploradorDePlanalto
    {
        public readonly QuebraDeEspeficacao RegrasNegocio = new QuebraDeEspeficacao();
        private RegraDeNegocio violacaoDeRegra;
        private Coordenada coordenada;
        private Posicao posicaoInicioalDaSonda;
        private DirecaoCardinal direcaoCardinalInicioalDaSonda;
        IMovimento movimentoSempreParaFrente;
        private string[] serieDeInstrucoesIndicandoParaASondaComoElaDeveraExplorarOPlanalto;
        private readonly IConexaoComOBanco ConexaoComOBanco;
        private readonly IMongoDatabase BancoDeDados;
        private string resultado;
        private int numeroDeLinhasNaMensagemEnviadaParaControlarAsSondas = 5;

        public ExploradorDePlanalto(IMongoDatabase bandoDeDados, IConexaoComOBanco conexaoComOBanco)
        {
            ConexaoComOBanco = conexaoComOBanco;
            BancoDeDados = bandoDeDados;
        }

        public string Iniciar(string mensagem)
        {
            if (string.IsNullOrWhiteSpace(mensagem))
            {
                violacaoDeRegra = new RegraDeNegocio("Mensagem inválida.");
                RegrasNegocio.Adicionar(violacaoDeRegra);
            }

            string[] separadores = new string[] { "\n" };
            string[] linhas = mensagem.Split(separadores, StringSplitOptions.None);

            if (linhas.Length < numeroDeLinhasNaMensagemEnviadaParaControlarAsSondas)
            {
                violacaoDeRegra = new RegraDeNegocio($"Mensagem inválida, só contém {linhas.Length} linha(s).");
                RegrasNegocio.Adicionar(violacaoDeRegra);
            }

            if (!RegrasNegocio.HouveViolacao())
            {
                TratarLinhas(linhas);
            }

            return resultado;
        }

        private void TratarLinhas(string[] linhas)
        {
            var sondaNumero = 1;
            var contardorDeLinhas = 1;

            foreach (var linha in linhas)
            {
                switch (contardorDeLinhas)
                {
                    case 1:
                        ObterCoordenadaDoPontoSuperiorDireitoDaMalhaDoPlanalto(linha);
                        break;
                    case 4:
                    case 2:
                        ObterPosicaoInicialDaSonda(linha);
                        break;
                    case 5:
                    case 3:
                        ObterSerieDeInstrucoesIndicandoParaASondaComoElaDeveraExplorarOPlanalto(linha);
                        break;
                }
                if (RegrasNegocio.HouveViolacao())
                {
                    break;
                }

                if (contardorDeLinhas == 3 || contardorDeLinhas == 5)
                {
                    ExecutarExploracao(sondaNumero);
                    sondaNumero++;
                }

                contardorDeLinhas++;
            }
        }

        private void ExecutarExploracao(int sondaNumero)
        {
            Sondas sondas = new Sondas(BancoDeDados);

            Planalto planalto = new Planalto();
            planalto.Criar(coordenada);

            movimentoSempreParaFrente = new MovimentoParaFrente();

            var nomeDaSonda = $"Mark {sondaNumero}";

            Sonda sonda;

            sonda = sondas.ObterPorNome(nomeDaSonda);

            if (sonda == null)
            {
                sonda = new Sonda(nomeDaSonda);
            }

            sonda.Explorar(planalto);

            sonda.IniciarEm(posicaoInicioalDaSonda, direcaoCardinalInicioalDaSonda);

            ExecutarInstrucaoNaSonda(sonda, movimentoSempreParaFrente);

            sondas.Gravar(sonda);

            sondas = null;

            var direcao = sonda.DirecaoAtual.ToString().ToUpper().Substring(0, 1).Replace("O", "W").Replace("L", "E");

            if (sondaNumero > 1)
            {
                resultado += "-";
            }

            resultado += $"{sonda.PosicaoAtual.X} {sonda.PosicaoAtual.Y} {direcao}";
        }

        private void ExecutarInstrucaoNaSonda(Sonda sonda, IMovimento movimentoSempreParaFrente)
        {
            for (int contador = 0; contador < serieDeInstrucoesIndicandoParaASondaComoElaDeveraExplorarOPlanalto.Length; contador++)
            {
                var instrucao = serieDeInstrucoesIndicandoParaASondaComoElaDeveraExplorarOPlanalto[contador];

                switch (instrucao)
                {
                    case "L":
                        sonda.Vire(Direcao.Esqueda);
                        break;
                    case "R":
                        sonda.Vire(Direcao.Direita);
                        break;
                    case "M":
                        sonda.Move(movimentoSempreParaFrente);
                        break;
                }
            }
        }

        private void ObterSerieDeInstrucoesIndicandoParaASondaComoElaDeveraExplorarOPlanalto(string linha)
        {
            if (linha.Length < 1)
            {
                violacaoDeRegra = new RegraDeNegocio($"Mensagem inválida, série de instruções indicando para a sonda como ela deverá explorar o planalto só contém {linha.Length} caracter(s).");
                RegrasNegocio.Adicionar(violacaoDeRegra);
            }
            else
            {
                var quantidadeDeCaracteres = linha.Length;
                serieDeInstrucoesIndicandoParaASondaComoElaDeveraExplorarOPlanalto = new string[quantidadeDeCaracteres];
                for (int contador = 0; contador < linha.Length; contador++)
                {
                    serieDeInstrucoesIndicandoParaASondaComoElaDeveraExplorarOPlanalto[contador] = linha[contador].ToString();
                }
            }
        }

        private void ObterPosicaoInicialDaSonda(string linha)
        {
            var caracteres = ObterCaracteresSeparadosPorEspaco(linha);

            if (caracteres.Length <= 2)
            {
                violacaoDeRegra = new RegraDeNegocio($"Mensagem inválida, posição inicial só contém {caracteres.Length} caracter(s).");
                RegrasNegocio.Adicionar(violacaoDeRegra);
            }
            else
            {
                int[] numeros = new int[2];
                string[] letras = new string[1];

                int contador = 0;
                foreach (var item in caracteres)
                {
                    int numero = 0;
                    char letrar = ' ';

                    switch (contador)
                    {
                        case 0:
                        case 1:
                            if (!ENumero(item, out numero))
                            {
                                violacaoDeRegra = new RegraDeNegocio("Mensagem inválida, posição inicial não contém valores númericos.");
                                RegrasNegocio.Adicionar(violacaoDeRegra);
                            }
                            else
                            {
                                numeros[contador] = numero;
                            }
                            break;
                        case 2:
                            letrar = Convert.ToChar(item);
                            if (!char.IsLetter(letrar))
                            {
                                violacaoDeRegra = new RegraDeNegocio("Mensagem inválida, posição inicial não contém caracter.");
                                RegrasNegocio.Adicionar(violacaoDeRegra);
                            }
                            else
                            {
                                letras[0] = item;
                            }
                            break;
                    }
                    contador++;
                }

                if (!RegrasNegocio.HouveViolacao())
                {
                    posicaoInicioalDaSonda = new Posicao(numeros[0], numeros[1]);
                    direcaoCardinalInicioalDaSonda = ObterDirecaoCardinal(letras[0]);
                }
            }
        }

        private DirecaoCardinal ObterDirecaoCardinal(string letra)
        {
            DirecaoCardinal direcaoCardinal = DirecaoCardinal.Norte;

            switch (letra)
            {
                case "E":
                    direcaoCardinal = DirecaoCardinal.Leste;
                    break;
                case "S":
                    direcaoCardinal = DirecaoCardinal.Sul;
                    break;
                case "W":
                    direcaoCardinal = DirecaoCardinal.Oeste;
                    break;
                default:
                    break;
            }

            return direcaoCardinal;
        }

        public void ObterCoordenadaDoPontoSuperiorDireitoDaMalhaDoPlanalto(string linha)
        {
            var caracteres = ObterCaracteresSeparadosPorEspaco(linha);

            if (caracteres.Length <= 1)
            {
                violacaoDeRegra = new RegraDeNegocio($"Mensagem inválida, coordenada do ponto superior-direito da malha do planalto só contém {caracteres.Length} caracter(s).");
                RegrasNegocio.Adicionar(violacaoDeRegra);
            }
            else
            {
                int[] numeros = new int[2];
                int contador = 0;
                foreach (var item in caracteres)
                {
                    int numero = 0;

                    if (!ENumero(item, out numero))
                    {
                        violacaoDeRegra = new RegraDeNegocio("Mensagem inválida, coordenada do ponto superior-direito da malha do planalto não contém valores númericos.");
                        RegrasNegocio.Adicionar(violacaoDeRegra);
                        break;
                    }

                    numeros[contador] = numero;
                    contador++;
                }

                if (!RegrasNegocio.HouveViolacao())
                {
                    coordenada = new Coordenada(numeros[0], numeros[1]);
                }
            }
        }

        private bool ENumero(string item, out int numero)
        {
            return (Int32.TryParse(item, out numero));
        }

        private string[] ObterCaracteresSeparadosPorEspaco(string linha)
        {
            return linha.Split(' ');
        }
    }
}
