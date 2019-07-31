using System;
using System.Collections.Generic;
using tabuleiro;
using xadrez;

namespace XadrezConsole {
    class Tela {

        public static void imprimirPartida(PartidaDeXadrez partida) {
            imprimirTabuleiro(partida.tab);
            Console.WriteLine();
            imprimirCapturadas(partida);
            Console.WriteLine("Turno: " + partida.turno);
            if (!partida.terminada) {
                Console.WriteLine("Aguardando jogada: " + partida.jogadorAtual);
                if (partida.xeque) {
                    Console.WriteLine("XEQUE!");
                }
            } else {
                Console.WriteLine("XEQUEMATE!");
                Console.WriteLine("Vencedor: " + partida.jogadorAtual);
            }
        }

        public static void imprimirCapturadas(PartidaDeXadrez partida) {
            Console.WriteLine("Peças capturadas:");
            Console.Write("Brancas: ");
            ConsoleColor aux = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Green;
            imprimirConjunto(partida.capturadas(Cor.Branca));
            Console.ForegroundColor = aux;
            Console.Write("Pretas: ");
            Console.ForegroundColor = ConsoleColor.Red;
            imprimirConjunto(partida.capturadas(Cor.Preta));
            Console.ForegroundColor = aux;
            Console.WriteLine();
        }

        public static void imprimirConjunto(HashSet<Peca> conjunto) {
            Console.Write("[");
            foreach (Peca p in conjunto) {
                Console.Write(p + ", ");
            }
            Console.WriteLine("]");
        }

        //Imprime o Tabuleiro sem mostrar os movimentos possíveis
        public static void imprimirTabuleiro(Tabuleiro tab) {

            for (int i = 0; i < tab.linhas; i++) {
                Console.Write(8 - i + " ");
                for (int j = 0; j < tab.colunas; j++) {
                    imprimirPeca(tab.peca(i, j));
                }
                Console.WriteLine();
            }
            Console.WriteLine("  a b c d e f g h");
        }

        //Imprime o Tabuleiro mostrando os movimentos possíveis
        public static void imprimirTabuleiro(Tabuleiro tab, bool[,] posicoePossiveis) {

            ConsoleColor fundoOriginal = Console.BackgroundColor;
            ConsoleColor fundoAlterado = ConsoleColor.DarkGray;

            for (int i = 0; i < tab.linhas; i++) {
                Console.Write(8 - i + " ");
                for (int j = 0; j < tab.colunas; j++) {
                    if (posicoePossiveis[i, j]) {
                        Console.BackgroundColor = fundoAlterado;
                    } else {
                        Console.BackgroundColor = fundoOriginal;
                    }
                    imprimirPeca(tab.peca(i, j));
                    Console.BackgroundColor = fundoOriginal;
                }
                Console.WriteLine();
            }
            Console.WriteLine("  a b c d e f g h");
            Console.BackgroundColor = fundoOriginal;
        }

        //Imprime a peça ou um '-' se a posição estiver vazia
        public static void imprimirPeca(Peca peca) {
            if (peca == null) {
                Console.Write("- ");
            } else {
                if (peca.cor == Cor.Branca) {
                    ConsoleColor aux = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(peca);
                    Console.ForegroundColor = aux;
                } else {
                    ConsoleColor aux = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(peca);
                    Console.ForegroundColor = aux;
                }
                Console.Write(" ");
            }
        }

        //Lê a coordenada da jogada
        public static PosicaoXadrez lerPosicaoXadrez() {
            try {
                string s = Console.ReadLine();
                char coluna = s[0];
                int linha = int.Parse(s[1] + "");
                return new PosicaoXadrez(coluna, linha);
            } catch {
                throw new TabuleiroException("Nenhuma posição informada");
            }
        }
    }
}
