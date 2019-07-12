using System;
using System.Collections.Generic;
using System.Text;

namespace tabuleiro {
    class Tabuleiro {
        public int linhas { get; set; }
        public int colunas { get; set; }
        private Peca[,] pecas;

        public Tabuleiro(int linhas, int colunas) {
            this.linhas = linhas;
            this.colunas = colunas;
            pecas = new Peca[linhas, colunas];
        }

        //Retorna a peça da posição informada
        public Peca peca(int linha, int coluna) {
            return pecas[linha, coluna];
        }

        //Retorna a peça da posição informada
        public Peca peca(Posicao pos) {
            return pecas[pos.linha, pos.coluna];
        }

        //Coloca a peça na posição informada
        public void colocarPeca(Peca p, Posicao pos) {
            if (existePeca(pos)) {
                throw new TabuleiroException("Já existe uma peça nessa posição!");
            }
            pecas[pos.linha, pos.coluna] = p;
            p.posicao = pos;
        }

        //Retira a peça da posição informada
        public Peca retirarPeca(Posicao pos) {
            if (peca(pos) == null) {
                return null;
            }
            Peca aux = peca(pos);
            aux.posicao = null;
            pecas[pos.linha, pos.coluna] = null;
            return aux;
        }

        //Verifica se existe alguma peça na posição
        public bool existePeca(Posicao pos) {
            validarPosicao(pos);
            return peca(pos) != null;
        }

        //Verifica se a posição é válida de acordo com o tamanho do tabuleiro
        public bool posicaoValida(Posicao pos) {
            if (pos.linha < 0 || pos.linha >= linhas || pos.coluna < 0 || pos.coluna >= colunas) {
                return false;
            }
            return true;
        }

        //Valida a posição
        public void validarPosicao(Posicao pos) {
            if (!posicaoValida(pos)) {
                throw new TabuleiroException("Posição inválida!");
            }
        }
    }
}
