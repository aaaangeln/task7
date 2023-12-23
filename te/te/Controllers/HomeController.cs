using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using te.Hubs;
using te.Models;

namespace te.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHubContext<ChatHub> _hubContext;

        public HomeController(IHubContext<ChatHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public IActionResult Index()
        {
            TicTacToeModel model = new TicTacToeModel
            {
                Board = new string[3, 3] { { "", "", "" }, { "", "", "" }, { "", "", "" } },
                CurrentPlayer = "X"
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult MakeMove(int row, int col, [Bind("Board,CurrentPlayer")] TicTacToeModel model)
        {
            if (model.Board[row, col] == "")
            {
                model.Board[row, col] = model.CurrentPlayer;

                if (CheckForWinner(model.Board, model.CurrentPlayer))
                {
                    model.Winner = model.CurrentPlayer;
                    model.CurrentPlayer = "";
                }
                else if (CheckForDraw(model.Board))
                {
                    model.Winner = "Draw";
                    model.CurrentPlayer = "";
                }
                else
                {
                    model.CurrentPlayer = (model.CurrentPlayer == "X") ? "O" : "X";
                }

                _hubContext.Clients.All.SendAsync("UpdateGame", model);

                return Ok();
            }

            return BadRequest();
        }

        private bool CheckForWinner(string[,] board, string currentPlayer)
        {
            for (int row = 0; row < 3; row++)
            {
                if (board[row, 0] == currentPlayer && board[row, 1] == currentPlayer && board[row, 2] == currentPlayer)
                {
                    return true;
                }
            }
            for (int col = 0; col < 3; col++)
            {
                if (board[0, col] == currentPlayer && board[1, col] == currentPlayer && board[2, col] == currentPlayer)
                {
                    return true;
                }
            }
            if (board[0, 0] == currentPlayer && board[1, 1] == currentPlayer && board[2, 2] == currentPlayer)
            {
                return true;
            }
            if (board[0, 2] == currentPlayer && board[1, 1] == currentPlayer && board[2, 0] == currentPlayer)
            {
                return true;
            }

            return false;
        }

        private bool CheckForDraw(string[,] board)
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (board[row, col] == "")
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}