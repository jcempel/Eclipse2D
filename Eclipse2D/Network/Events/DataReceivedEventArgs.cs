using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace Eclipse2D.Network.Events
{
    /// <summary>
    /// Represents a network session that has received data.
    /// </summary>
    public class DataReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// Represents the socket associated with this event.
        /// </summary>
        private Socket m_Socket;

        /// <summary>
        /// Represents the packet received by the network server.
        /// </summary>
        private Byte[] m_Packet;

        /// <summary>
        /// Initializes a new DataReceivedEventArgs with the specified socket and packet.
        /// </summary>
        /// <param name="Socket">The socket associated with this event.</param>
        /// <param name="Packet">The packet associated with this event.</param>
        public DataReceivedEventArgs(Socket Socket, Byte[] Packet)
        {
            m_Socket = Socket;
            m_Packet = Packet;
        }

        /// <summary>
        /// Gets the socket associated with this event.
        /// </summary>
        public Socket Socket
        {
            get
            {
                return m_Socket;
            }
        }

        /// <summary>
        /// Gets the packet associated with this event.
        /// </summary>
        public Byte[] Packet
        {
            get
            {
                return m_Packet;
            }
        }

        /// <summary>
        /// Gets the length, in bytes, of the packet.
        /// </summary>
        public Int32 Length
        {
            get
            {
                return m_Packet.Length;
            }
        }
    }
}
