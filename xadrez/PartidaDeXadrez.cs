using System.Collections.Generic;
using tabuleiro;


namespace xadrez {
    class PartidaDeXadrez {

        public Tabuleiro tab { get; private set; }
        public int turno { get; private set; }
        public Cor jogadorAtual { get; private set; }
        public bool terminada { get; private set; }
        private HashSet<Peca> pecas;
        private HashSet<Peca> pecasCapturadas;

        public PartidaDeXadrez() {
            tab = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branca;
            terminada = false;
            pecas = new HashSet<Peca>();
            pecasCapturadas = new HashSet<Peca>();
            colocarPecas();
        }

        //Movimenta a peça e captura a adversária quando necessário
        public void executaMovimento(Posicao origem, Posicao destino) {
            Peca p = tab.retirarPeca(origem);
            p.incrementarQteMovimentos();
            Peca pecaCapturada = tab.retirarPeca(destino);
            tab.colocarPeca(p, destino);
            if (pecaCapturada != null) {
                pecasCapturadas.Add(pecaCapturada);
            }
        }

        //Executa uma jogada movendo a peça e alterando o turno dos jogadores
        public void realizaJogada(Posicao origem, Posicao destino) {
            executaMovimento(origem, destino);
            turno++;
            mudaJogador();
        }

        //Muda o turno dos jogadores
        private void mudaJogador() {
            if (jogadorAtual == Cor.Branca) {
                jogadorAtual = Cor.Preta;
            } else {
                jogadorAtual = Cor.Branca;
            }
        }

        //Obtem todas as peças capturadas de uma determinada cor
        public HashSet<Peca> capturadas(Cor cor) {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca p in pecasCapturadas) {
                if (p.cor == cor) {
                    aux.Add(p);
                }
            }
            return aux;
        }

        //Obtem todas as peças em jogo de uma determinada cor
        public HashSet<Peca> emJogo(Cor cor) {
            HashSet<Peca> aux = new HashSet<Peca>();
            foreach (Peca p in pecas) {
                if (p.cor == cor) {
                    aux.Add(p);
                }
            }
            aux.ExceptWith(capturadas(cor));
            return aux;
        }

        //Verifica se a posição de origem da peça é válida
        public void validaOrigem(Posicao posicao) {
            if (tab.peca(posicao) == null) {
                throw new TabuleiroException("Não existe peça na posição de origem escolhida");
            }
            if (jogadorAtual != tab.peca(posicao).cor) {
                throw new TabuleiroException("A peça de origem escolhida não é " + jogadorAtual);
            }
            if (!tab.peca(posicao).existemMovimentosPossiveis()) {
                throw new TabuleiroException("Não há movimentos possíveis para esta peça");
            }
        }


        //Verifica se a posição de destino da peça é válida
        public void validaDestino(Posicao origem, Posicao destino) {
            if (!tab.peca(origem).podeMoverPara(destino)) {
                throw new TabuleiroException("Posição de destino inválida");
            }
        }

        //Coloca uma nova peça na posição informada
        public void colocarNovaPeca(char coluna, int linha, Peca peca) {
            tab.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        //Coloca as peças no tabuleiro  
        private void colocarPecas() {
            colocarNovaPeca('c', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('c', 2, new Torre(tab, Cor.Branca));
            colocarNovaPeca('d', 2, new Torre(tab, Cor.Branca));
            colocarNovaPeca('e', 2, new Torre(tab, Cor.Branca));
            colocarNovaPeca('e', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('d', 1, new Rei(tab, Cor.Branca));

            colocarNovaPeca('c', 7, new Torre(tab, Cor.Preta));
            colocarNovaPeca('c', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('d', 7, new Torre(tab, Cor.Preta));
            colocarNovaPeca('e', 7, new Torre(tab, Cor.Preta));
            colocarNovaPeca('e', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('d', 8, new Rei(tab, Cor.Preta  ));
        }
    }
}