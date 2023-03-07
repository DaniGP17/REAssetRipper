using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using REAssetRipper.Core.Constants;

namespace REAssetRipper.UnrealEngine
{
    public class Connection
    {
        public void Connect(string host, int port, Sockets.types protocol)
        {
            if(protocol == Sockets.types.TCP)
            {
                try
                {
                    TcpClient tcpCliente = new TcpClient();
                    tcpCliente.Connect(host, port);
                    Console.WriteLine("Conectado a {0} en el puerto {1} mediante TCP", host, port);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Error al conectarse a {0} en el puerto {1} mediante TCP: {2}", host, port, ex.Message);
                }
            }
            else if(protocol == Sockets.types.UDP)
            {
                try
                {
                    UdpClient udpCliente = new UdpClient();
                    udpCliente.Connect(host, port);
                    Console.WriteLine("Conectado a {0} en el puerto {1} mediante UDP", host, port);
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("Error al conectarse a {0} en el puerto {1} mediante UDP: {2}", host, port, ex.Message);
                }
            }
        }
    }
}
