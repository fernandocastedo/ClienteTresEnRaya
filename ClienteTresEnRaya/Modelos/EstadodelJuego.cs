using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteTresEnRaya.Modelos
{
    public class EstadodelJuego
    {
        public string[] Board { get; } = new string[9];   // "", "X" u "O"
        public string MySymbol { get; set; } = "";
        public event Action? BoardChanged;

        public bool SetCell(int idx, string symbol)
        {
            if (idx is < 0 or > 8 || !string.IsNullOrEmpty(Board[idx])) return false;
            Board[idx] = symbol;
            BoardChanged?.Invoke();
            return true;
        }

        public void LoadFromServer(string[] snapshot)
        {
            Array.Copy(snapshot, Board, 9);
            BoardChanged?.Invoke();
        }
    }
}
