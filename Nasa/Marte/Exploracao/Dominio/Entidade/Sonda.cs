﻿using Marte.Exploracao.Dominio.Contratos;
using Marte.Exploracao.Dominio.ObjetoDeValor;
using System;
using System.Collections.Generic;

namespace Marte.Exploracao.Dominio.Entidade
{
    public class Sonda
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public Planalto Planalto { get; private set; }
        public Posicao PosicaoAtual { get; private set; }
        public DirecaoCardinal DirecaoAtual { get; private set; }
        public readonly IEspecificacaoDeNegocio EspecificacaoDeNegocio = new EspecificacaoDeNegocio();
        private readonly IDictionary<Direcao, Action> movimentosExploratorio;
        private readonly IDictionary<DirecaoCardinal, Action> direcaoSentidoHorario;
        private readonly IDictionary<DirecaoCardinal, Action> direcaoSentidoAntiHorario;

        private Sonda()
        {
            movimentosExploratorio = new Dictionary<Direcao, Action>
            {
                {Direcao.Direita, () => direcaoSentidoHorario[DirecaoAtual].Invoke()},
                {Direcao.Esqueda, () => direcaoSentidoAntiHorario[DirecaoAtual].Invoke()}
            };

            direcaoSentidoHorario = new Dictionary<DirecaoCardinal, Action>
            {
                {DirecaoCardinal.Norte, () => DirecaoAtual = DirecaoCardinal.Leste},
                { DirecaoCardinal.Leste, () => DirecaoAtual = DirecaoCardinal.Sul},
                {DirecaoCardinal.Sul, () => DirecaoAtual = DirecaoCardinal.Oeste},
                { DirecaoCardinal.Oeste, () => DirecaoAtual = DirecaoCardinal.Norte}
            };

            direcaoSentidoAntiHorario = new Dictionary<DirecaoCardinal, Action>
            {
                {DirecaoCardinal.Norte, () => DirecaoAtual = DirecaoCardinal.Oeste},
                {DirecaoCardinal.Oeste, () => DirecaoAtual = DirecaoCardinal.Sul},
                {DirecaoCardinal.Sul, () => DirecaoAtual = DirecaoCardinal.Leste},
                { DirecaoCardinal.Leste, () => DirecaoAtual = DirecaoCardinal.Norte}
            };
        }

        public Sonda(string nome) : this()
        {
            if (string.IsNullOrWhiteSpace(nome))
            {
                EspecificacaoDeNegocio.Adicionar(new RegraDeNegocio("O nome da sonda não foi informado."));
                return;
            }

            Nome = nome;

        }

        public void Explorar(Planalto planalto)
        {
            if (planalto == null)
            {
                EspecificacaoDeNegocio.Adicionar(new RegraDeNegocio("O planalto a ser explorado não foi informado."));
                return;
            }

            Planalto = planalto;
        }

        public void IniciarEm(Posicao posicaoDesejada, DirecaoCardinal direcaoCardinal)
        {
            if (posicaoDesejada == null)
            {
                EspecificacaoDeNegocio.Adicionar(new RegraDeNegocio("A posição inicial da sonda não foi informado."));
                return;
            }
            else
            {
                if (posicaoDesejada.X > Planalto.EixoX() | posicaoDesejada.Y > Planalto.EixoY())
                {
                    EspecificacaoDeNegocio.Adicionar(new RegraDeNegocio("Posição fora da faixa (Malha do Planalto) para exploração."));
                    return;
                }
            }

            PosicaoAtual = posicaoDesejada;
            DirecaoAtual = direcaoCardinal;

        }

        public void Vire(Direcao movimento)
        {
            movimentosExploratorio[movimento].Invoke();
        }

        public void Move(IMovimento movimento)
        {
            PosicaoAtual = movimento.Executar(DirecaoAtual, PosicaoAtual);
        }

        public bool MeusDadosSaoValidos()
        {
            if (string.IsNullOrWhiteSpace(Nome))
            {
                EspecificacaoDeNegocio.Adicionar(new RegraDeNegocio("O nome da sonda não foi informado."));
            }

            if (Planalto == null)
            {
                EspecificacaoDeNegocio.Adicionar(new RegraDeNegocio("O planalto a ser explorado não foi informado."));
            }

            if (PosicaoAtual == null)
            {
                EspecificacaoDeNegocio.Adicionar(new RegraDeNegocio("A posição inicial da sonda não foi informado."));
            }

            return EspecificacaoDeNegocio.HouveViolacao();
        }
    }
}

