using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
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
    /// Interaction logic for UserControlOnline.xaml
    /// </summary>
    public partial class UserControlOnline : UserControl
    {
        private TcpClient cliente;
        private StreamWriter writer;
        private StreamReader reader;
        private Thread hiloRecepcion;
        private Button[] celdas = new Button[9];
        private string miSimbolo = "";

        public UserControlOnline()
        {
            InitializeComponent();
            InicializarTablero();
        }

        private void InicializarTablero()
        {
            BoardGrid.Children.Clear();
            celdas = new Button[9];

            for (int i = 0; i < 9; i++)
            {
                var btn = new Button
                {
                    Tag = i,
                    FontSize = 48,
                    Margin = new Thickness(5),
                    Background = new SolidColorBrush(Color.FromRgb(255, 107, 107)),
                    Foreground = Brushes.White,
                    FontWeight = FontWeights.Bold
                };
                btn.Click += BtnCelda_Click;
                BoardGrid.Children.Add(btn);
                celdas[i] = btn;
            }
        }


        private void BtnCelda_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int pos = (int)btn.Tag;

            if (!string.IsNullOrEmpty((string)btn.Content))
                return;

            if (string.IsNullOrEmpty(miSimbolo))
            {
                MessageBox.Show("Aún no estás conectado o asignado como jugador.");
                return;
            }

            writer.WriteLine(pos);
            writer.Flush();
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                cliente = new TcpClient(HostBox.Text.Trim(), int.Parse(PortBox.Text.Trim()));
                NetworkStream stream = cliente.GetStream();
                writer = new StreamWriter(stream) { AutoFlush = true };
                reader = new StreamReader(stream);

                hiloRecepcion = new Thread(RecibirMensajes) { IsBackground = true };
                hiloRecepcion.Start();

                StatusTxt.Text = "Conectado, esperando asignación...";
                ConnectBtn.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al conectar: " + ex.Message);
            }
        }

        private void RecibirMensajes()
        {
            try
            {
                while (true)
                {
                    string mensaje = reader.ReadLine();
                    if (mensaje == null) break;

                    if (mensaje.StartsWith("Bienvenido") || mensaje.StartsWith("Comienza"))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            StatusTxt.Text = mensaje;
                            if (mensaje.Contains(" X")) miSimbolo = "X";
                            else if (mensaje.Contains(" O")) miSimbolo = "O";
                        });
                    }
                    else if (mensaje.StartsWith("Fin:") || mensaje.StartsWith("Tablero reiniciado"))
                    {
                        Dispatcher.Invoke(() => StatusTxt.Text = mensaje);
                    }
                    else
                    {
                        var estadoTablero = mensaje.Split(' ');
                        Dispatcher.Invoke(() => RefreshBoard(estadoTablero));
                    }
                }
            }
            catch (IOException)
            {
                Dispatcher.Invoke(() => StatusTxt.Text = "Desconectado del servidor.");
            }
        }

        private void RefreshBoard(string[] board)
        {
            for (int i = 0; i < 9; i++)
            {
                celdas[i].Content = board.Length > i && !string.IsNullOrEmpty(board[i]) ? board[i] : "";
            }
        }
    }
}
