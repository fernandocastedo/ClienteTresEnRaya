using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteTresEnRaya.Vistas
{
    public interface IGameView
    {
        /// <summary>Se dispara cuando el usuario hace clic en una celda (0-8).</summary>
        event Action<int> CellClicked;

        /// <summary>Pinta el tablero completo usando el snapshot recibido.</summary>
        void RefreshBoard(string[] board);

        /// <summary>Muestra mensajes de estado (turno, errores, etc.).</summary>
        void ShowStatus(string message);
    }
}
