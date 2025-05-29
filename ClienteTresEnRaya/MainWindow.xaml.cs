using ClienteTresEnRaya.Red;
using ClienteTresEnRaya.Modelos;
using ClienteTresEnRaya.Controlador;
using ClienteTresEnRaya.Vistas;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClienteTresEnRaya
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IGameView
    {
        public event Action<int>? CellClicked;

        private readonly EstadodelJuego _state = new();
        private readonly NetworkClient _net = new();
        private GameController? _controller;

        public MainWindow()
        {
            InitializeComponent();
            BuildBoard();
        }

        #region IGameView
        public void RefreshBoard(string[] board)
        {
            for (int i = 0; i < 9; i++)
                ((Button)BoardGrid.Children[i]).Content = board[i];
        }

        public void ShowStatus(string msg) => StatusTxt.Text = msg;
        #endregion

        private void BuildBoard()
        {
            for (int i = 0; i < 9; i++)
            {
                var btn = new Button { FontSize = 32, Tag = i };
                btn.Click += (_, _) => CellClicked?.Invoke((int)btn.Tag);
                BoardGrid.Children.Add(btn);
            }
        }

        private async void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string host = HostBox.Text.Trim();
                if (!int.TryParse(PortBox.Text.Trim(), out int port))
                    throw new Exception("Puerto inválido");

                ConnectBtn.IsEnabled = false;
                ShowStatus("Conectando…");
                await _net.ConnectAsync(host, port);        // ⟵ ahora recibe puerto
                _controller = new GameController(_state, this, _net);
            }
            catch (Exception ex)
            {
                ShowStatus($"Error: {ex.Message}");
                ConnectBtn.IsEnabled = true;
            }
        }
    }
}