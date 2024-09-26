using System;
using System.Collections.Generic;
using System.Linq;

namespace Wipeseals
{
    public class KnuckleBonesCore
    {

        public enum Result
        {
            Player0Win,
            Player1Win,
            Draw,
        }
        #region Constants
        /// <summary>
        /// The number of players in the game.
        /// </summary>
        public const int PLAYER_NUM = 2;

        /// <summary>
        /// The width of the game board.
        /// </summary>
        public const int BOARD_WIDTH = 3;

        /// <summary>
        /// The depth of the game board.
        /// </summary>
        public const int BOARD_DEPTH = 3;

        /// <summary>
        /// The minimum dice number.
        /// </summary>
        public const int DICE_MIN = 1;

        /// <summary>
        /// The maximum dice number.
        /// </summary>
        public const int DICE_MAX = 6;

        #endregion

        #region Fields
        /// <summary>
        /// The game board is a 3x3 grid.
        /// </summary>
        public int?[,,] GameBoard { get => _gameBoard; internal set => _gameBoard = value; }

        /// <summary>
        /// The current player.
        /// </summary>
        public int CurrentPlayer { get => _currentPlayer; internal set => _currentPlayer = value; }

        /// <summary>
        /// The turn number.
        /// </summary>
        public int Turn { get => _turn; internal set => _turn = value; }
        #endregion

        #region Private Fields
        /// <summary>
        /// The game board is a 3x3 grid. 
        /// </summary>
        private int?[,,] _gameBoard = new int?[PLAYER_NUM, BOARD_WIDTH, BOARD_DEPTH];

        /// <summary>
        /// The current player.
        /// </summary>
        private int _currentPlayer = 0;

        /// <summary>
        /// The turn number.
        /// </summary>
        private int _turn = 1;

        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random _random = new Random();
        #endregion

        public KnuckleBonesCore()
        {
            Reset();
        }

        /// <summary>
        /// Reset the game board.
        /// </summary>
        public void Reset()
        {
            for (int i = 0; i < PLAYER_NUM; i++)
            {
                for (int j = 0; j < BOARD_WIDTH; j++)
                {
                    for (int k = 0; k < BOARD_DEPTH; k++)
                    {
                        GameBoard[i, j, k] = null;
                    }
                }
            }

            Turn = 1;
            CurrentPlayer = new Random().Next(0, PLAYER_NUM);
        }

        /// <summary>
        /// Place counter on the game board.
        /// </summary>
        /// <param name="player">player id</param>
        public int GetDiceCount(int player)
        {
            int count = 0;
            for (int i = 0; i < BOARD_WIDTH; i++)
            {
                for (int j = 0; j < BOARD_DEPTH; j++)
                {
                    if (GameBoard[player, i, j] != null)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        /// <summary>
        /// Get the score for a row.
        /// </summary>
        /// <param name="player">player id</param>
        /// <param name="y">y coordinate</param>
        /// <returns>score</returns>
        public int GetColumnScore(int player, int x)
        {
            int score = 0;
            // 同一列のダイスで同じ値のものがあれば、その数だけスコアに乗算
            for (int diceNumber = DICE_MIN; diceNumber <= DICE_MAX; diceNumber++)
            {
                int count = Enumerable.Range(0, BOARD_DEPTH)
                                      .Select(i => GameBoard[player, x, i] ?? 0) // 0はないのでnullの場合は0に変換
                                      .Count(i => i == diceNumber);
                score += count * (count * diceNumber); // (count * diceNumber) が元の値。 count乗算分がボーナスでの倍額
            }
            return score;
        }

        /// <summary>
        /// Get the player's score.
        /// </summary>
        /// <param name="player">player id</param>
        /// <returns>score</returns>
        public int GetPlayerScore(int player)
        {
            int score = Enumerable.Range(0, BOARD_WIDTH)
                                  .Select(i => GetColumnScore(player, i))
                                  .Sum();
            return score;
        }

        /// <summary>
        /// Game over.
        /// </summary>
        public bool IsGameOver()
        {
            for (int i = 0; i < PLAYER_NUM; i++)
            {
                // どちらかのPlayerのすべての列にダイスが配置された場合、ゲーム終了
                if (GetDiceCount(i) == BOARD_WIDTH * BOARD_DEPTH)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Get the winner.
        /// </summary>
        /// <returns>result</returns>
        public Result? GetWinner()
        {
            if (!IsGameOver())
            {
                return null;
            }

            int player0Score = GetPlayerScore(0);
            int player1Score = GetPlayerScore(1);

            if (player0Score > player1Score)
            {
                return Result.Player0Win;
            }
            else if (player0Score < player1Score)
            {
                return Result.Player1Win;
            }
            else
            {
                return Result.Draw;
            }
        }

        /// <summary>
        /// Arrange the players.
        /// </summary>
        public void ArrangePlayers()
        {
            CurrentPlayer = (CurrentPlayer + 1) % PLAYER_NUM;
        }

        /// <summary>
        /// Roll the dice.
        /// </summary>
        public int RollDice()
        {
            return _random.Next(DICE_MIN, DICE_MAX + 1);
        }


        /// <summary>
        /// Put the dice on the game board.
        /// </summary>
        /// <param name="player">player id</param>
        /// <param name="diceNumber">dice number</param>
        /// <param name="x">x coordinate</param>
        /// <returns>put succeed</returns>
        public bool PutDice(int player, int diceNumber, int x)
        {
            // depthは浅い順に詰めて配置
            bool isPut = false;
            for (int i = 0; i < BOARD_DEPTH; i++)
            {
                if (GameBoard[player, x, i] == null)
                {
                    GameBoard[player, x, i] = diceNumber;
                    isPut = true;
                    break;
                }
            }
            // おけない場合はfalseを返す
            if (!isPut)
            {
                return false;
            }

            // 相手Playerの同一列のダイスを取り除いて再配置
            var opponent = (player + 1) % PLAYER_NUM;
            var opponentColumn = Enumerable.Range(0, BOARD_DEPTH)
                                           .Select(i => GameBoard[opponent, x, i])
                                           .Where(i => i != null && i != diceNumber)
                                           .ToArray();
            for (int i = 0; i < BOARD_DEPTH; i++)
            {
                GameBoard[opponent, x, i] = (i < opponentColumn.Length) ? opponentColumn[i] : null;
            }

            // 次のPlayerに交代
            ArrangePlayers();
            Turn++;

            return true;
        }
    }
}