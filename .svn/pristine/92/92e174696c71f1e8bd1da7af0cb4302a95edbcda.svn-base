using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Eclipse2D.Network.Packets;

namespace Eclipse2D.Network
{
    /// <summary>
    /// Represents a network client, which can asynchronously send and receive data.
    /// </summary>
    public class NetworkClient
    {
        private SocketAsyncEventArgs m_SendEvent;

        private SocketAsyncEventArgs m_ReceiveEvent;

        private Int32 m_BufferSize;

        private Boolean m_BuildPackets;

        private IPEndPoint m_IPEndPoint;

        private Boolean m_IsRunning;

        public NetworkClient(Int32 BufferSize, Boolean BuildPackets)
        {
            m_BufferSize = BufferSize;

            m_BuildPackets = BuildPackets;

            m_SendEvent = new SocketAsyncEventArgs();
            m_SendEvent.SetBuffer(new Byte[BufferSize], 0, BufferSize);
            m_SendEvent.Completed += SocketEvent_IOCompleted;

            m_ReceiveEvent = new SocketAsyncEventArgs();
            m_ReceiveEvent.SetBuffer(new Byte[BufferSize], 0, BufferSize);
            m_ReceiveEvent.Completed += SocketEvent_IOCompleted;

            m_IsRunning = false;
        }

        public void Connect(String Address, Int32 Port)
        {

        }

        public void Disconnect()
        {

        }

        private void SocketEvent_IOCompleted(Object Sender, SocketAsyncEventArgs SocketEvent)
        {
            switch (SocketEvent.LastOperation)
            {
                case SocketAsyncOperation.Connect:
                    Client_EndConnect(SocketEvent);
                    break;

                case SocketAsyncOperation.Send:
                    Client_EndSend(SocketEvent);
                    break;

                case SocketAsyncOperation.Receive:
                    Client_EndReceive(SocketEvent);
                    break;
            }
        }

        private void Client_BeginConnect(SocketAsyncEventArgs ConnectEvent)
        {

        }

        private void Client_EndConnect(SocketAsyncEventArgs ConnectEvent)
        {

        }

        private void Client_BeginSend(SocketAsyncEventArgs SendEvent)
        {

        }

        private void Client_EndSend(SocketAsyncEventArgs SendEvent)
        {

        }

        private void Client_BeginReceive(SocketAsyncEventArgs ReceiveEvent)
        {

        }

        private void Client_EndReceive(SocketAsyncEventArgs ReceiveEvent)
        {

        }

        public static Byte[] CreatePacket(PacketWriter Writer)
        {
            Byte[] PacketData = new Byte[4 + Writer.Length];

            Byte[] LengthHeader = BitConverter.GetBytes(Writer.Length);

            Buffer.BlockCopy(LengthHeader, 0, PacketData, 0, 4);
            Buffer.BlockCopy(Writer.ToNetworkBuffer(), 0, PacketData, 4, Writer.Length);

            return PacketData;
        }
    }
}
