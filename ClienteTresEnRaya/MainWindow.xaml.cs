using ClienteTresEnRaya.Vistas;
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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnOnline_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UserControlOnline();
        }

        private void BtnMaquina_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new UserControlMaquina();
        }
    }
}
