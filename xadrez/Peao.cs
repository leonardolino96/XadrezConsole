using tabuleiro;

namespace xadrez {
    class Peao : Peca {
        public Peao(Tabuleiro tab, Cor cor) : base(tab, cor) {
        }

        //Verifica se a posição no Tabuleiro está vazia ou possui uma peça de OUTRA cor
        private bool podeMover(Posicao pos) {
            Peca p = tab.peca(pos);
            return p == null || p.cor != cor;
        }

        private bool existeInimigo(Posicao pos) {
            Peca p = tab.peca(pos);
            return p != null && p.cor != cor;
        }

        private bool livre(Posicao pos) {
            return tab.peca(pos) == null;
        }

        //Mapeia todas as posições possíveis para a peça
        public override bool[,] movimentosPossiveis() {
            bool[,] mat = new bool[tab.linhas, tab.colunas];

            Posicao pos = new Posicao(0, 0);

            if (cor == Cor.Branca) {
                pos.definirValores(posicao.linha - 1, posicao.coluna);
                if (tab.posicaoValida(pos) && podeMover(pos)) {
                    mat[pos.linha, pos.coluna] = true;
                }
                
                pos.definirValores(posicao.linha - 2, posicao.coluna);
                if (tab.posicaoValida(pos) && podeMover(pos) && qteMovimentos == 0) {
                    mat[pos.linha, pos.coluna] = true;
                }

                pos.definirValores(posicao.linha - 1, posicao.coluna - 1);
                if (tab.posicaoValida(pos) && podeMover(pos) && existeInimigo(pos)) {
                    mat[pos.linha, pos.coluna] = true;
                }

                pos.definirValores(posicao.linha - 1, posicao.coluna + 1);
                if (tab.posicaoValida(pos) && podeMover(pos) && existeInimigo(pos)) {
                    mat[pos.linha, pos.coluna] = true;
                }
            } else {
                pos.definirValores(posicao.linha + 1, posicao.coluna);
                if (tab.posicaoValida(pos) && podeMover(pos)) {
                    mat[pos.linha, pos.coluna] = true;
                }
                
                pos.definirValores(posicao.linha + 2, posicao.coluna);
                if (tab.posicaoValida(pos) && podeMover(pos) && qteMovimentos == 0) {
                    mat[pos.linha, pos.coluna] = true;
                }

                pos.definirValores(posicao.linha + 1, posicao.coluna - 1);
                if (tab.posicaoValida(pos) && podeMover(pos) && existeInimigo(pos)) {
                    mat[pos.linha, pos.coluna] = true;
                }

                pos.definirValores(posicao.linha + 1, posicao.coluna + 1);
                if (tab.posicaoValida(pos) && podeMover(pos) && existeInimigo(pos)) {
                    mat[pos.linha, pos.coluna] = true;
                }
            }



            return mat;
        }

        public override string ToString() {
            return "P";
        }
    }
}
