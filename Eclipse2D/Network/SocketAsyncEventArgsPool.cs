using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Eclipse2D.Network
{
    /// <summary>
    /// Represents a thread-safe pool of re-useable SocketAsyncEventArg events.
    /// </summary>
    public class SocketAsyncEventArgsPool
    {
        /// <summary>
        /// Represents a pool of re-useable
        /// </summary>
        private Stack<SocketAsyncEventArgs> m_SocketEventPool;

        /// <summary>
        /// Represents the lock used for the socket pool.
        /// </summary>
        private Object m_SocketPoolLock = new Object();

        /// <summary>
        /// Initializes a new SocketAsyncEventArgsPool with the specified capacity.
        /// </summary>
        /// <param name="Capacity">The capacity of the SocketAsyncEventArgsPool.</param>
        public SocketAsyncEventArgsPool(Int32 Capacity)
        {
            // Initialize the socket event pool.
            m_SocketEventPool = new Stack<SocketAsyncEventArgs>(Capacity);
        }

        /// <summary>
        /// Adds a SocketAsyncEventArgs event to the pool.
        /// </summary>
        /// <param name="Item">The SocketAsyncEventArgs to add.</param>
        public void Push(SocketAsyncEventArgs Item)
        {
            // Check if the SocketAsyncEventArgs being added is NULL.
            if (Item == null)
            {
                throw new ArgumentNullException("The socket event being added to the SocketAsyncEventArgsPool cannot be NULL.");
            }

            // Lock the SocketAsyncEventArgsPool object.
            lock (m_SocketPoolLock)
            {
                // Add the SocketAsyncEventArgs event to the pool.
                m_SocketEventPool.Push(Item);
            }
        }

        /// <summary>
        /// Removes a SocketAsyncEventArgs event from the pool and returns it.
        /// </summary>
        /// <returns></returns>
        public SocketAsyncEventArgs Pop()
        {
            // Lock the SocketAsyncEventArgsPool object.
            lock (m_SocketPoolLock)
            {
                if (m_SocketEventPool.Count == 0)
                {
                    throw new InvalidOperationException("Cannot remove the socket event because no socket events exist.");
                }

                // Get the SocketAsyncEventArgs event from the pool.
                return m_SocketEventPool.Pop();
            }
        }

        /// <summary>
        /// Gets the number of available socket events currently in the pool.
        /// </summary>
        public Int32 Count
        {
            get
            {
                lock (m_SocketPoolLock)
                {
                    return m_SocketEventPool.Count;
                }
            }
        }

    }
}
