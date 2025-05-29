using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ClienteTresEnRaya
{
    public partial class MainWindow : Window
    {
        private TcpClient? cliente;
        private StreamWriter? writer;
        private StreamReader? reader;
        private Thread? hiloRecepcion;
        private Button[] celdas = new Button[9];
        private string miSimbolo = "";

        public MainWindow()
        {
            InitializeComponent();
            InicializarTablero();
        }

        private void InicializarTablero()
        {
            for (int i = 0; i < 9; i++)
            {
                Button btn = new Button
                {
                    FontSize = 32,
                    Tag = i,
                    Margin = new Thickness(5)
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
                return; // La celda ya está ocupada

            if (string.IsNullOrEmpty(miSimbolo))
            {
                MessageBox.Show("Aún no estás conectado o asignado como jugador.");
                return;
            }

            if (writer == null)
            {
                MessageBox.Show("No se pudo enviar el movimiento, no hay conexión.");
                return;
            }

            try
            {
                writer.WriteLine(pos);
                writer.Flush();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error enviando el movimiento: " + ex.Message);
            }
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(HostBox.Text) || string.IsNullOrWhiteSpace(PortBox.Text))
                {
                    MessageBox.Show("Por favor ingresa la IP y el puerto.");
                    return;
                }

                cliente = new TcpClient();
                cliente.Connect(HostBox.Text.Trim(), int.Parse(PortBox.Text.Trim()));
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
                StatusTxt.Text = "Error de conexión";
            }
        }

        private void RecibirMensajes()
        {
            try
            {
                while (true)
                {
                    string? mensaje = reader?.ReadLine();
                    if (mensaje == null)
                        break;

                    if (mensaje.StartsWith("Bienvenido") || mensaje.StartsWith("Comienza"))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            StatusTxt.Text = mensaje;
                            if (mensaje.Contains(" X"))
                                miSimbolo = "X";
                            else if (mensaje.Contains(" O"))
                                miSimbolo = "O";
                        });
                    }
                    else if (mensaje.StartsWith("Fin:") || mensaje.StartsWith("Tablero reiniciado"))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            StatusTxt.Text = mensaje;
                        });
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
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => StatusTxt.Text = "Error: " + ex.Message);
            }
        }

        private void RefreshBoard(string[] board)
        {
            for (int i = 0; i < 9; i++)
            {
                string celda = board.Length > i ? board[i] : "";
                celdas[i].Content = string.IsNullOrEmpty(celda) ? "" : celda;
            }
        }
    }
}
