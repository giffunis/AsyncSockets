using System;
using System.Text;
using Sockets;
using Sockets.EventArgs;

namespace AsyncSockets
{
    class AsyncClientSocket
    {

        private ClientSocket _mySock;
        private byte[] _receiveBuffer = new byte[0];
        private string _data;

        public AsyncClientSocket(string endpoint, int port)
        {
            _mySock = new ClientSocket(endpoint, port);
            _mySock.Connected += MySockOnConnected;
            _mySock.Disconnected += MySockOnDisconnected;
            _mySock.DataReceived += MySockOnDataReceived;
        }

        public void Connect(string data)
        {
            _data = data;
            _mySock.Connect();
        }

        private void MySockOnConnected(SocketConnectedArgs args)
        {
            WriteToBox("Connected to Server!");

            _mySock.Send(Encoding.ASCII.GetBytes("NICK u24something\r\n"));
            _mySock.Send(Encoding.ASCII.GetBytes("USER u24something u24something bla :u24something\r\n"));
            _mySock.Send(Encoding.ASCII.GetBytes("MODE u24something +B-x\r\n"));
            //Task.Run(() => ParseLoop());
        }

        private void MySockOnDataReceived(DataReceivedArgs args)
        {
            lock (_receiveBuffer)
            {
                if (_receiveBuffer.Length == 0)
                {
                    _receiveBuffer = args.Data;
                    return;
                }

                var tempBuff = new byte[_receiveBuffer.Length + args.Data.Length];
                Buffer.BlockCopy(_receiveBuffer, 0, tempBuff, 0, _receiveBuffer.Length);
                Buffer.BlockCopy(args.Data, 0, tempBuff, _receiveBuffer.Length, args.Data.Length);
                _receiveBuffer = tempBuff;
            }
        }

        private void MySockOnDisconnected(SocketDisconnectedArgs args)
        {
            WriteToBox(" **** Disconnected ! **** ");
        }

        private void WriteToBox(string message)
        {
            Console.WriteLine(message);
        }
    }
}
