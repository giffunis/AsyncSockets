using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Sockets;
using Sockets.EventArgs;

namespace AsyncSockets
{
    class AsyncServerSocket
    {

        TcpListener _listener;
        private List<ClientSocket> _sockets = new List<ClientSocket>();

        public AsyncServerSocket(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
        }

        public AsyncServerSocket(IPAddress ipAddress, int port)
        {
            _listener = new TcpListener(ipAddress, port);
        }

        public void StartListening()
        {
            _listener.Start();
            AcceptFunction();
            WriteToBox("Listening");
        }

        private void AcceptFunction()
        {
            _listener.BeginAcceptTcpClient(AcceptCallback, null);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            var client = _listener.EndAcceptTcpClient(ar);
            var newSocket = new ClientSocket();
            newSocket.DataReceived += SocketOnDataReceived;
            newSocket.Disconnected += SocketOnDisconnected;
            newSocket.Accept(client);

            WriteToBox("Accepted Client!!!");
            AcceptFunction();
        }

        private void SocketOnDataReceived(DataReceivedArgs args)
        {
            WriteToBox("Data Received");
            var readBuffer = args.Data;
            int bytesRead = readBuffer.Length;

            // There  might be more data, so store the data received so far.  
            StringBuilder sb = new StringBuilder();
            sb.Append(Encoding.ASCII.GetString(
                readBuffer, 0, bytesRead));

            String content = sb.ToString();
            Console.WriteLine("Read {0} bytes from socket. \n Data : {1}",
                content.Length, content);

        }

        private void SocketOnDisconnected(SocketDisconnectedArgs args)
        {
            WriteToBox("Client Disconnected!!!");
            _sockets.Remove(args.BaseSocket);
        }

        private void WriteToBox(string message)
        {
            Console.WriteLine(message);
        }
    }
}





