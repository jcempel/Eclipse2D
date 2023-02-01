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
    /// Represents a network session that has connected.
    /// </summary>
    public class ConnectedEventArgs : EventArgs
    {
        /// <summary>
        /// Represents the socket associated with this event.
        /// </summary>
        private Socket m_Socket;

        /// <summary>
        /// Initializes a new ConnectEventArgs with the specified socket.
        /// </summary>
        /// <param name="Socket">The socket associated with the event.</param>
        public ConnectedEventArgs(Socket Socket)
        {
            m_Socket = Socket;
        }

        /// <summary>
        /// Gets the connected socket.
        /// </summary>
        public Socket Socket
        {
            get
            {
                return m_Socket;
            }
        }

        /// <summary>
        /// Gets the IP address of the connected socket.
        /// </summary>
        public String Address
        {
            get
            {
                return ((IPEndPoint)m_Socket.RemoteEndPoint).Address.ToString();
            }
        }

        /// <summary>
        /// Gets the port of the connected socket.
        /// </summary>
        public Int32 Port
        {
            get
            {
                return ((IPEndPoint)m_Socket.RemoteEndPoint).Port;
            }
        }

        /// <summary>
        /// Gets the address family of the connected socket.
        /// </summary>
        public AddressFamily AddressFamily
        {
            get
            {
                return ((IPEndPoint)m_Socket.RemoteEndPoint).AddressFamily;
            }
        }

        /// <summary>
        /// Gets if the socket is using IP version 4.
        /// </summary>
        public Boolean IsIPv4Address
        {
            get
            {
                return ((IPEndPoint)m_Socket.RemoteEndPoint).AddressFamily == AddressFamily.InterNetwork;
            }
        }

        /// <summary>
        /// Gets if the socket is using IP version 6.
        /// </summary>
        public Boolean IsIPv6Address
        {
            get
            {
                return ((IPEndPoint)m_Socket.RemoteEndPoint).AddressFamily == AddressFamily.InterNetworkV6;
            }
        }    
    }
}
