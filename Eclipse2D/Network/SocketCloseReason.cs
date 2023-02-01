using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eclipse2D.Network
{
    /// <summary>
    /// Represents the reason why a socket was closed.
    /// </summary>
    public enum SocketCloseReason
    {
        /// <summary>
        /// Represents the socket was closed for unknown reasons.
        /// </summary>
        Unknown,

        /// <summary>
        /// Represents the socket was successfully closed by the network server.
        /// </summary>
        ServerShutdown,

        /// <summary>
        /// Represents the socket was successfully closed remotely.
        /// </summary>
        RemotelyClosed,

        /// <summary>
        /// Represents the socket was closed because of a socket error.
        /// </summary>
        SocketError,

        /// <summary>
        /// Represents the socket was closed because the network session was invalid.
        /// </summary>
        SessionNull
    }
}
