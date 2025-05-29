using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClienteTresEnRaya.Red
{
    public class NetworkClient
    {
        private TcpClient _client = null!;
        private StreamWriter _w = null!;
        private StreamReader _r = null!;
        public event Action<string>? MessageReceived;

        public async Task ConnectAsync(string host, int port = 12345)
        {
            _client = new TcpClient();
            await _client.ConnectAsync(host, port);
            var s = _client.GetStream();
            _w = new StreamWriter(s) { AutoFlush = true };
            _r = new StreamReader(s);
            _ = ListenLoop();                     // fire-and-forget
        }

        public void SendMove(int cell) => _w.WriteLine(cell);

        private async Task ListenLoop()
        {
            string? line;
            while ((line = await _r.ReadLineAsync()) is not null)
                MessageReceived?.Invoke(line);
        }
    }
}
