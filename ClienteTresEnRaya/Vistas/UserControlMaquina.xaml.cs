using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClienteTresEnRaya.Vistas
{
    /// <summary>
    /// Interaction logic for UserControlMaquina.xaml
    /// </summary>
    public partial class UserControlMaquina : UserControl
    {
        private Button[] celdas = new Button[9];
        private string jugador = "X";
        private string maquina = "O";
        private bool turnoJugador = true;
        private Random random = new Random();

        public UserControlMaquina()
        {
            InitializeComponent();
            InicializarTablero();
            StatusTxt.Text = "Tu turno (X)";
        }

        private void InicializarTablero()
        {
            for (int i = 0; i < 9; i++)
            {
                Button btn = new Button { Tag = i, FontSize = 32, Margin = new Thickness(5) };
                btn.Click += BtnCelda_Click;
                BoardGrid.Children.Add(btn);
                celdas[i] = btn;
            }
        }

        private async void BtnCelda_Click(object sender, RoutedEventArgs e)
        {
            if (!turnoJugador) return;

            Button btn = sender as Button;
            int pos = (int)btn.Tag;

            if (!string.IsNullOrEmpty((string)btn.Content)) return;

            btn.Content = jugador;

            if (ChequeaVictoria(jugador))
            {
                StatusTxt.Text = "Ganaste! 🎉";
                turnoJugador = false;
                return;
            }

            if (TableroLleno())
            {
                StatusTxt.Text = "Empate!";
                turnoJugador = false;
                return;
            }

            turnoJugador = false;
            StatusTxt.Text = "Turno máquina (O)";

            // Esperar que UI refresque
            await Task.Delay(300);

            TurnoMaquina();
        }


        private void TurnoMaquina()
        {
            var libres = new List<int>();
            for (int i = 0; i < 9; i++)
                if (string.IsNullOrEmpty(celdas[i].Content as string))
                    libres.Add(i);

            if (libres.Count == 0) return;

            int pos = libres[random.Next(libres.Count)];
            celdas[pos].Content = maquina;

            if (ChequeaVictoria(maquina))
            {
                StatusTxt.Text = "La máquina gana 😢";
                turnoJugador = false;
                return;
            }

            if (TableroLleno())
            {
                StatusTxt.Text = "Empate!";
                turnoJugador = false;
                return;
            }

            turnoJugador = true;
            StatusTxt.Text = "Tu turno (X)";
        }

        private bool ChequeaVictoria(string jugador)
        {
            int[][] combinacionesGanadoras = new int[][]
            {
                new int[] { 0, 1, 2 },
                new int[] { 3, 4, 5 },
                new int[] { 6, 7, 8 },
                new int[] { 0, 3, 6 },
                new int[] { 1, 4, 7 },
                new int[] { 2, 5, 8 },
                new int[] { 0, 4, 8 },
                new int[] { 2, 4, 6 }
            };

            foreach (var c in combinacionesGanadoras)
            {
                var a = celdas[c[0]].Content as string;
                var b = celdas[c[1]].Content as string;
                var c2 = celdas[c[2]].Content as string;

                if (!string.IsNullOrWhiteSpace(a) &&
                    !string.IsNullOrWhiteSpace(b) &&
                    !string.IsNullOrWhiteSpace(c2) &&
                    a == jugador &&
                    b == jugador &&
                    c2 == jugador)
                {
                    return true;
                }
            }
            return false;
        }

        private bool TableroLleno()
        {
            return celdas.All(c => !string.IsNullOrEmpty((string)c.Content));
        }

        private void BtnNuevoJuego_Click(object sender, RoutedEventArgs e)
        {
            foreach (var btn in celdas)
                btn.Content = "";
            turnoJugador = true;
            StatusTxt.Text = "Tu turno (X)";
        }
    }
}
