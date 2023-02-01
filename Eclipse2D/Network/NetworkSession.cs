using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Eclipse2D.Network
{
    /// <summary>
    /// Represents a network session attached to a socket event.
    /// </summary>
    public class NetworkSession
    {
        /// <summary>
        /// Represents the socket associated with this network session.
        /// </summary>
        private Socket m_Socket;

        /// <summary>
        /// Represents the network server associated with this network session.
        /// </summary>
        private NetworkServer m_NetworkServer;

        /// <summary>
        /// Represents the index of this sessions created by the network server.
        /// </summary>
        private Int32 m_SessionIndex;

        /// <summary>
        /// Represents a queue of buffers that have been received.
        /// </summary>
        private Queue<Byte[]> m_ReceiveQueue;

        /// <summary>
        /// Represents a queue of buffers that have yet to be sent.
        /// </summary>
        private Queue<Byte[]> m_SendQueue;

        /// <summary>
        /// Represents the receive buffer, which processes in-complete buffers.
        /// </summary>
        private Byte[] m_ReceiveBuffer;

        /// <summary>
        /// Represents the send buffer, which processes in-complete buffers.
        /// </summary>
        private Byte[] m_SendBuffer;

        /// <summary>
        /// Represents a signal which prevents multiple threads from attempting to send data at the same time.
        /// </summary>
        private SemaphoreSlim m_SendSignal;

        /// <summary>
        /// Represents the amount of bytes received by this socket.
        /// </summary>
        private Int32 m_BytesReceived;

        /// <summary>
        /// Represents the amount of bytes sent by this socket.
        /// </summary>
        private Int32 m_BytesSent;

        /// <summary>
        /// Initializes a new NetworkSession.
        /// </summary>
        /// <param name="Server">The network server that created the network session.</param>
        /// <param name="Socket">The socket associated with the network session.</param>
        public NetworkSession(NetworkServer Server, Int32 SessionIndex, Socket Socket)
        {
            // Initialize the server.
            m_NetworkServer = Server;

            // Initialize the socket.
            m_Socket = Socket;

            // Set the session index.
            m_SessionIndex = SessionIndex;

            // Initialize the receive buffers.
            m_ReceiveQueue = new Queue<Byte[]>();

            // Initialize the send buffers.
            m_SendQueue = new Queue<Byte[]>();

            // Initialize the receive buffer.
            m_ReceiveBuffer = new Byte[0];

            // Initialize the send buffer.
            m_SendBuffer = new Byte[0];

            // Initialize the session send signal.
            m_SendSignal = new SemaphoreSlim(1);

            // Set the bytes received to zero.
            m_BytesReceived = 0;

            // Set the bytes sent to zero.
            m_BytesSent = 0;
        }

        /// <summary>
        /// Adds data to the sending queue.
        /// </summary>
        /// <param name="SendData">The data to be added to the sending queue.</param>
        public void SendBytes(Byte[] SendData)
        {
            m_NetworkServer.SendBytes(this, SendData);
        }

        /// <summary>
        /// Processes an asynchronous receive event and transforms the data into a completed byte array.
        /// </summary>
        /// <param name="ReceiveEvent">The receive event to process.</param>
        public void ProcessReceive(SocketAsyncEventArgs ReceiveEvent)
        {
            // Create the local buffer to hold new incoming data.
            Byte[] LocalBuffer = new Byte[m_ReceiveBuffer.Length + ReceiveEvent.BytesTransferred];

            // Copy the existing byte buffer to our temporary one.
            Buffer.BlockCopy(m_ReceiveBuffer, 0, LocalBuffer, 0, m_ReceiveBuffer.Length);

            // Copy the new data to the end of the temporary buffer.
            Buffer.BlockCopy(ReceiveEvent.Buffer, ReceiveEvent.Offset, LocalBuffer, m_ReceiveBuffer.Length, ReceiveEvent.BytesTransferred);

            // Increment the amount of bytes received on this network session.
            m_BytesReceived += ReceiveEvent.BytesTransferred;

            // Replace the buffer with the new local buffer.
            m_ReceiveBuffer = LocalBuffer;

            // Each packet is defined with a length before the packet header. The length is defined
            // as an Int32, so we need at least 4 bytes to determine if a potential packet exists.
            while (m_ReceiveBuffer.Length >= 4)
            {
                // Get the length of the packet.
                Int32 BufferLen = BitConverter.ToInt32(m_ReceiveBuffer, 0);

                // We have a full packet, now we can extract the data.
                if (m_ReceiveBuffer.Length < 4 + BufferLen)
                    break;

                // Re-create the local buffer to hold our new data.
                Byte[] PayloadBuffer = new Byte[BufferLen];

                // Copy the actual packet data (excluding the length) to the local byte array.
                Buffer.BlockCopy(m_ReceiveBuffer, 4, PayloadBuffer, 0, BufferLen);

                // Enqueue the payload buffer into the received buffers.
                m_ReceiveQueue.Enqueue(PayloadBuffer);

                // Get the new length, excluding the previous length and packet data.
                Int32 NewLen = m_ReceiveBuffer.Length - (4 + BufferLen);

                // Create a new buffer to hold the data.
                LocalBuffer = new Byte[NewLen];

                // Copy the buffer to the local buffer, excluding the previous length and packet data.
                Buffer.BlockCopy(m_ReceiveBuffer, 4 + BufferLen, LocalBuffer, 0, NewLen);

                // Replace the receive buffer with the new local buffer.
                m_ReceiveBuffer = LocalBuffer;
            }
        }

        /// <summary>
        /// Processes an asynchronous receive event and adds the raw contents to the buffer queue.
        /// </summary>
        /// <param name="ReceiveEvent">The receive event to process.</param>
        public void ProcessReceiveRaw(SocketAsyncEventArgs ReceiveEvent)
        {
            // Initialize the raw data buffer to match the size of the amount of bytes transferred.
            Byte[] RawDataBuffer = new Byte[ReceiveEvent.BytesTransferred];

            // Increment the amount of bytes received on this network session.
            m_BytesReceived += ReceiveEvent.BytesTransferred;

            // Copy the amount of bytes transferred from the receive event to the raw data buffer.
            Buffer.BlockCopy(ReceiveEvent.Buffer, ReceiveEvent.Offset, RawDataBuffer, 0, ReceiveEvent.BytesTransferred);

            // Enqueue the raw data buffer.
            m_ReceiveQueue.Enqueue(RawDataBuffer);
        }

        /// <summary>
        /// Processes an asynchronous send event, and de-queues data to the send buffer.
        /// </summary>
        /// <param name="SendEvent">The send event to process.</param>
        public void ProcessSend(SocketAsyncEventArgs SendEvent)
        {
            // Check if the send buffer is empty.
            if (m_SendBuffer.Length == 0)
            {
                // De-queues the next buffer into the send buffer.
                m_SendBuffer = m_SendQueue.Dequeue();
            }

            // Check if the contents of the send buffer can fit inside the socket buffer.
            if (m_SendBuffer.Length <= m_NetworkServer.BufferSize)
            {
                // Adjust the size of the socket buffer to only send the remaining bytes in the send buffer.
                SendEvent.SetBuffer(SendEvent.Offset, m_SendBuffer.Length);

                // Copy the send buffer to the socket buffer.
                Buffer.BlockCopy(m_SendBuffer, 0, SendEvent.Buffer, SendEvent.Offset, m_SendBuffer.Length);

                // Initialize a new send buffer indicating the processor is ready for the next queued send buffer.
                m_SendBuffer = new Byte[0];
            }
            else
            {
                // Adjust the size of the socket buffer to send the maximum amount of bytes allowed by the socket event.
                SendEvent.SetBuffer(SendEvent.Offset, m_NetworkServer.BufferSize);

                // Copy part of the send buffer to the socket buffer.
                Buffer.BlockCopy(m_SendBuffer, 0, SendEvent.Buffer, SendEvent.Offset, m_NetworkServer.BufferSize);

                // Initialize a new byte array to hold the remaining bytes of the send buffer.
                Byte[] LocalBuffer = new Byte[m_SendBuffer.Length - m_NetworkServer.BufferSize];

                // Copy the remaining bytes to the to the new byte array.
                Buffer.BlockCopy(m_SendBuffer, m_NetworkServer.BufferSize, LocalBuffer, 0, m_SendBuffer.Length - m_NetworkServer.BufferSize);

                // Replace the send buffer with the new local buffer.
                m_SendBuffer = LocalBuffer;
            }
        }

        /// <summary>
        /// Sets/Gets the socket associated with this network session.
        /// </summary>
        public Socket Socket
        {
            set
            {
                m_Socket = value;
            }

            get
            {
                return m_Socket;
            }
        }

        /// <summary>
        /// Gets the index of the network session.
        /// </summary>
        public Int32 SessionIndex
        {
            get
            {
                return m_SessionIndex;
            }
        }

        /// <summary>
        /// Gets the receive queue for the network session.
        /// </summary>
        public Queue<Byte[]> ReceiveQueue
        {
            get
            {
                return m_ReceiveQueue;
            }
        }

        /// <summary>
        /// Gets the send queue for the network session.
        /// </summary>
        public Queue<Byte[]> SendQueue
        {
            get
            {
                return m_SendQueue;
            }
        }

        /// <summary>
        /// Gets the send signal semaphore for the network session.
        /// </summary>
        public SemaphoreSlim SendSignal
        {
            get
            {
                return m_SendSignal;
            }
        }

        /// <summary>
        /// Gets the amount of bytes received.
        /// </summary>
        public Int32 BytesReceived
        {
            get
            {
                return m_BytesReceived;
            }
        }

        /// <summary>
        /// Gets the amount of bytes sent.
        /// </summary>
        public Int32 BytesSent
        {
            get
            {
                return m_BytesSent;
            }
        }

        /// <summary>
        /// Gets if data is available to be read.
        /// </summary>
        public Boolean DataAvailableForRead
        {
            get
            {
                return (m_ReceiveQueue.Count > 0);
            }
        }

        /// <summary>
        /// Gets if data is available to be written.
        /// </summary>
        public Boolean DataAvailableForWrite
        {
            get
            {
                return (m_SendBuffer.Length > 0 || m_SendQueue.Count > 0);
            }
        }
    }
}
