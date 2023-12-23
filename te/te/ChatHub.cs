using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using te.Models;

namespace te.Hubs
{
    public class ChatHub : Hub
    {
        private readonly TicTacToeModel _model;

        public ChatHub(TicTacToeModel model)
        {
            _model = model;
        }

        public async Task JoinGame(string groupId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupId);
        }

        public async Task MakeMove(int row, int col)
        {
            // Проверяем, что игра еще не завершена
            if (!string.IsNullOrEmpty(_model.Winner))
            {
                return;
            }

            // Проверяем, что клетка, на которую сделан ход, пуста
            if (!IsCellEmpty(row, col))
            {
                return;
            }

            // Заполняем клетку текущим игроком (X или O)
            _model.Board[row, col] = _model.CurrentPlayer;

            // Проверяем наличие победителя
            if (CheckForWinner(_model.CurrentPlayer))
            {
                _model.Winner = _model.CurrentPlayer;
            }
            // Проверяем наличие ничьей
            else if (IsBoardFull())
            {
                _model.Winner = "Draw";
            }
            else
            {
                // Меняем текущего игрока
                _model.CurrentPlayer = (_model.CurrentPlayer == "X") ? "O" : "X";
            }

            // Отправляем обновленную модель всем подключенным клиентам
            await Clients.Group("gameId").SendAsync("UpdateGame", _model);
        }

        private bool IsCellEmpty(int row, int col)
        {
            return string.IsNullOrEmpty(_model.Board[row, col]);
        }

        private bool IsBoardFull()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (string.IsNullOrEmpty(_model.Board[row, col]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private bool CheckForWinner(string player)
        {
            // Проверяем строки
            for (int row = 0; row < 3; row++)
            {
                if (_model.Board[row, 0] == player && _model.Board[row, 1] == player && _model.Board[row, 2] == player)
                {
                    return true;
                }
            }

            // Проверяем столбцы
            for (int col = 0; col < 3; col++)
            {
                if (_model.Board[0, col] == player && _model.Board[1, col] == player && _model.Board[2, col] == player)
                {
                    return true;
                }
            }

            // Проверяем диагонали
            if (_model.Board[0, 0] == player && _model.Board[1, 1] == player && _model.Board[2, 2] == player)
            {
                return true;
            }
            if (_model.Board[0, 2] == player && _model.Board[1, 1] == player && _model.Board[2, 0] == player)
            {
                return true;
            }

            return false;
        }
    }
}