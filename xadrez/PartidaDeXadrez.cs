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
        public bool xeque { get; private set; }
        public Peca vulneravelEnPassant { get; private set; }


        public PartidaDeXadrez() {
            tab = new Tabuleiro(8, 8);
            turno = 1;
            jogadorAtual = Cor.Branca;
            terminada = false;
            xeque = false;
            vulneravelEnPassant = null;
            pecas = new HashSet<Peca>();
            pecasCapturadas = new HashSet<Peca>();
            colocarPecas();
        }

        //Movimenta a peça e captura a adversária quando necessário
        public Peca executaMovimento(Posicao origem, Posicao destino) {
            Peca p = tab.retirarPeca(origem);
            p.incrementarQteMovimentos();
            Peca pecaCapturada = tab.retirarPeca(destino);
            tab.colocarPeca(p, destino);
            if (pecaCapturada != null) {
                pecasCapturadas.Add(pecaCapturada);
            }

            // #JogadaEspecial Roque pequeno
            if (p is Rei && destino.coluna == origem.coluna + 2) {
                Posicao origemTorre = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoTorre = new Posicao(origem.linha, origem.coluna + 1);
                Peca t = tab.retirarPeca(origemTorre);
                t.incrementarQteMovimentos();
                tab.colocarPeca(t, destinoTorre);
            }

            // #JogadaEspecial Roque grande
            if (p is Rei && destino.coluna == origem.coluna - 2) {
                Posicao origemTorre = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoTorre = new Posicao(origem.linha, origem.coluna - 1);
                Peca t = tab.retirarPeca(origemTorre);
                t.incrementarQteMovimentos();
                tab.colocarPeca(t, destinoTorre);
            }

            //#JogadaEspecial En passant
            if (p is Peao) {
                if (origem.coluna != destino.coluna && pecaCapturada == null) {
                    Posicao posPeao;
                    if (p.cor == Cor.Branca) {
                        posPeao = new Posicao(destino.linha + 1, destino.coluna);
                    } else {
                        posPeao = new Posicao(destino.linha - 1, destino.coluna);
                    }
                    pecaCapturada = tab.retirarPeca(posPeao);
                    pecasCapturadas.Add(pecaCapturada);
                }
            }
            return pecaCapturada;
        }

        public void desfazMovimento(Posicao origem, Posicao destino, Peca capturada) {
            Peca p = tab.retirarPeca(destino);
            p.decrementarQteMovimentos();
            if (capturada != null) {
                tab.colocarPeca(capturada, destino);
                pecasCapturadas.Remove(capturada);
            }
            tab.colocarPeca(p, origem);

            // #JogadaEspecial Roque pequeno
            if (p is Rei && destino.coluna == origem.coluna + 2) {
                Posicao origemTorre = new Posicao(origem.linha, origem.coluna + 3);
                Posicao destinoTorre = new Posicao(origem.linha, origem.coluna + 1);
                Peca t = tab.retirarPeca(destinoTorre);
                t.decrementarQteMovimentos();
                tab.colocarPeca(t, origemTorre);
            }

            // #JogadaEspecial Roque grande
            if (p is Rei && destino.coluna == origem.coluna - 2) {
                Posicao origemTorre = new Posicao(origem.linha, origem.coluna - 4);
                Posicao destinoTorre = new Posicao(origem.linha, origem.coluna - 1);
                Peca t = tab.retirarPeca(destinoTorre);
                t.decrementarQteMovimentos();
                tab.colocarPeca(t, origemTorre);
            }

            // #JogadaEspecial En Passant
            if (p is Peao) {
                if (origem.coluna != destino.coluna && capturada == vulneravelEnPassant) {
                    Peca peao = tab.retirarPeca(destino);
                    Posicao posPeao;
                    if (p.cor == Cor.Branca) {
                        posPeao = new Posicao(3, destino.coluna);
                    } else {
                        posPeao = new Posicao(4, destino.coluna);
                    }
                    tab.colocarPeca(peao, posPeao);
                }
            }
        }

        //Executa uma jogada movendo a peça e alterando o turno dos jogadores
        public void realizaJogada(Posicao origem, Posicao destino) {
            Peca pecaCapturada = executaMovimento(origem, destino);

            if (estaEmXeque(jogadorAtual)) {
                desfazMovimento(origem, destino, pecaCapturada);
                throw new TabuleiroException("Você não pode se colocar em Xeque");
            }

            if (estaEmXeque(adversaria(jogadorAtual))) {
                xeque = true;
            } else {
                xeque = false;
            }

            if (xequeMate(adversaria(jogadorAtual))) {
                terminada = true;
            } else {
                turno++;
                mudaJogador();
            }

            //#JogadaEspecial En Passant
            Peca p = tab.peca(destino);
            if (p is Peao && (destino.linha == origem.linha -2 
            || destino.linha == origem.linha + 2)) {
                vulneravelEnPassant = p;
            } else {
                vulneravelEnPassant = null;
            }
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
            if (!tab.peca(origem).movimentoPossivel(destino)) {
                throw new TabuleiroException("Posição de destino inválida");
            }
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

        //Retorna a cor adversária
        private Cor adversaria(Cor cor) {
            if (cor == Cor.Branca) {
                return Cor.Preta;
            } else {
                return Cor.Branca;
            }
        }

        //Retorna o rei adaversário
        private Peca rei(Cor cor) {
            foreach (Peca p in emJogo(cor)) {
                if (p is Rei) {
                    return p;
                }
            }
            return null;
        }

        //Verifica se alguma peça pode capturar o rei
        public bool estaEmXeque(Cor cor) {
            Peca r = rei(cor);
            if (r == null) {
                throw new TabuleiroException("Não há um rei da cor " + cor + " no tabuleiro");
            }
            foreach (Peca p in emJogo(adversaria(cor))) {
                bool[,] mat = p.movimentosPossiveis();
                if (mat[r.posicao.linha, r.posicao.coluna]) {
                    return true;
                }
            }
            return false;
        }

        //Verifica a condição de xequemate
        public bool xequeMate(Cor cor) {
            if (!estaEmXeque(cor)) {
                return false;
            }
            foreach (Peca p in emJogo(cor)) {
                bool[,] mat = p.movimentosPossiveis();
                for (int i = 0; i < tab.linhas; i++) {
                    for (int j = 0; j < tab.colunas; j++) {
                        if (mat[i, j]) {
                            Posicao origem = p.posicao;
                            Posicao destino = new Posicao(i, j);
                            Peca pecaCapturada = executaMovimento(origem, destino);
                            bool testeXeque = estaEmXeque(cor);
                            desfazMovimento(origem, destino, pecaCapturada);
                            if (!testeXeque) {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        //Coloca uma nova peça na posição informada
        public void colocarNovaPeca(char coluna, int linha, Peca peca) {
            tab.colocarPeca(peca, new PosicaoXadrez(coluna, linha).toPosicao());
            pecas.Add(peca);
        }

        //Coloca as peças no tabuleiro  
        private void colocarPecas() {
            colocarNovaPeca('a', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('b', 1, new Cavalo(tab, Cor.Branca));
            colocarNovaPeca('c', 1, new Bispo(tab, Cor.Branca));
            colocarNovaPeca('d', 1, new Dama(tab, Cor.Branca));
            colocarNovaPeca('e', 1, new Rei(tab, Cor.Branca, this));
            colocarNovaPeca('f', 1, new Bispo(tab, Cor.Branca));
            colocarNovaPeca('g', 1, new Cavalo(tab, Cor.Branca));
            colocarNovaPeca('h', 1, new Torre(tab, Cor.Branca));
            colocarNovaPeca('a', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('b', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('c', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('d', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('e', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('f', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('g', 2, new Peao(tab, Cor.Branca, this));
            colocarNovaPeca('h', 2, new Peao(tab, Cor.Branca, this));

            colocarNovaPeca('a', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('b', 8, new Cavalo(tab, Cor.Preta));
            colocarNovaPeca('c', 8, new Bispo(tab, Cor.Preta));
            colocarNovaPeca('d', 8, new Dama(tab, Cor.Preta));
            colocarNovaPeca('e', 8, new Rei(tab, Cor.Preta, this));
            colocarNovaPeca('f', 8, new Bispo(tab, Cor.Preta));
            colocarNovaPeca('g', 8, new Cavalo(tab, Cor.Preta));
            colocarNovaPeca('h', 8, new Torre(tab, Cor.Preta));
            colocarNovaPeca('a', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('b', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('c', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('d', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('e', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('f', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('g', 7, new Peao(tab, Cor.Preta, this));
            colocarNovaPeca('h', 7, new Peao(tab, Cor.Preta, this));
        }
    }
}