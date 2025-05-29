using ClienteTresEnRaya.Red;
using ClienteTresEnRaya.Modelos;
using ClienteTresEnRaya.Vistas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteTresEnRaya.Controlador
{
    public class GameController
    {
        private readonly EstadodelJuego _state;
        private readonly IGameView _view;
        private readonly NetworkClient _net;

        public GameController(EstadodelJuego state, IGameView view, NetworkClient net)
        {
            _state = state;
            _view = view;
            _net = net;

            _view.CellClicked += OnCellClicked;
            _state.BoardChanged += () => _view.RefreshBoard(_state.Board);
            _net.MessageReceived += OnServerMessage;
        }

        private void OnCellClicked(int idx) => _net.SendMove(idx);

        private void OnServerMessage(string msg)
        {
            if (msg.StartsWith("Bienvenido") || msg.StartsWith("Comienza"))
            {
                _view.ShowStatus(msg);
                if (msg.Contains(" X")) _state.MySymbol = "X";
                if (msg.Contains(" O")) _state.MySymbol = "O";
                return;
            }

            if (msg.StartsWith("Movimiento inválido"))
            {
                _view.ShowStatus(msg);
                return;
            }

            // Snapshot del tablero
            var cells = msg.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            _state.LoadFromServer(cells);
        }
    }
}
