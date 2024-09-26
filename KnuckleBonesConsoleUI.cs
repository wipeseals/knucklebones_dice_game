using System;
using System.Collections.Generic;
using System.Linq;

namespace Wipeseals
{
    public class KnuckleBonesConsoleUI
    {
        public KnuckleBonesCore KnuckleBonesCore { get; internal set; }


        public KnuckleBonesConsoleUI()
        {
            KnuckleBonesCore = new KnuckleBonesCore();

            Console.WriteLine("Welcome to KnuckleBones!");
        }

        public void DisplayGameBoard()
        {
            Console.WriteLine($"Turn: {KnuckleBonesCore.Turn} Player: {KnuckleBonesCore.CurrentPlayer}");
            Console.WriteLine("=========================================");
            Console.WriteLine("Player1 Score: " + KnuckleBonesCore.GetPlayerScore(0));
            Console.WriteLine("Player2 Score: " + KnuckleBonesCore.GetPlayerScore(1));
            Console.WriteLine("=========================================");

            Console.WriteLine(" Player 1  | Player 2");
            Console.WriteLine("===========|===========");
            Console.WriteLine(" 0 | 1 | 2 | 0 | 1 | 2 ");
            Console.WriteLine("===========|===========");

            for (int depth = 0; depth < KnuckleBonesCore.BOARD_DEPTH; depth++)
            {
                Console.Write(" ");
                for (int player = 0; player < KnuckleBonesCore.PLAYER_NUM; player++)
                {
                    for (int x = 0; x < KnuckleBonesCore.BOARD_WIDTH; x++)
                    {
                        Console.Write(KnuckleBonesCore.GameBoard[player, x, depth] == null ? " " : KnuckleBonesCore.GameBoard[player, x, depth]);
                        Console.Write(" | ");
                    }
                }
                Console.WriteLine();
            }
        }

        public KnuckleBonesCore.Result? PlayGame()
        {
            KnuckleBonesCore.Reset();
            while (true)
            {
                DisplayGameBoard();

                var diceNumber = KnuckleBonesCore.RollDice();
                Console.WriteLine($"Player {KnuckleBonesCore.CurrentPlayer + 1} rolled a {diceNumber}.");
                Console.WriteLine("Enter the column number to place your piece: ");
                var input = Console.ReadLine();
                if (input == null)
                {
                    Console.WriteLine("Input cannot be null. Try again.");
                    continue;
                }
                var column = int.Parse(input);

                if (!KnuckleBonesCore.PutDice(KnuckleBonesCore.CurrentPlayer, diceNumber, column))
                {
                    Console.WriteLine("Invalid move. Try again.");
                    continue;
                }
                if (KnuckleBonesCore.IsGameOver())
                {
                    // Game Over
                    DisplayGameBoard();

                    var result = KnuckleBonesCore.GetWinner();
                    if (result == KnuckleBonesCore.Result.Player0Win)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Player 1 wins!");
                    }
                    else if (result == KnuckleBonesCore.Result.Player1Win)
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Player 2 wins!");
                    }
                    else
                    {
                        Console.WriteLine("");
                        Console.WriteLine("Draw!");
                    }
                    return result;
                }
            }
        }

    }
}