using System;

namespace te.Models
{
    public class TicTacToeModel
    {
        public string[,] Board { get; set; }
        public string CurrentPlayer { get; set; }
        public string Winner { get; set; }

        public TicTacToeModel()
        {
            Board = new string[3, 3];
            CurrentPlayer = "X"; 
            Winner = "";
        }
    }
}