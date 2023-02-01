using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Eclipse2D.Network
{
    /// <summary>
    /// Represents a large byte buffer that can be divided for use with the SocketAsyncEventArgs classes.
    /// </summary>
    public class NetworkBufferManager
    {
        /// <summary>
        /// Represents the underlying byte array associated with the network buffer manager.
        /// </summary>
        private Byte[] m_ByteBuffer;

        /// <summary>
        /// Represents free buffer spaces in the byte buffer that are available to be re-used. 
        /// </summary>
        private Stack<Int32> m_FreeIndexPool;

        /// <summary>
        /// Represents the current buffer index when the byte buffer is being assigned for the first time.
        /// </summary>
        private Int32 m_CurrentIndex;

        /// <summary>
        /// Represents the size of each individual byte buffer.
        /// </summary>
        private Int32 m_IndividualBufferSize;

        /// <summary>
        /// Initializes a new NetworkBufferManager with the specified total buffer size, and individual buffer sizes.
        /// </summary>
        /// <param name="TotalBufferSize">The total size of the byte buffer.</param>
        /// <param name="IndividualBufferSize">The total size of each individual byte buffer.</param>
        /// <remarks>The TotalBufferSize must be divisible by the IndividualBufferSize or this call will fail.</remarks>
        public NetworkBufferManager(Int32 TotalBufferSize, Int32 IndividualBufferSize)
        {
            // Checks if we have valid buffer sizes.
            if ((TotalBufferSize % IndividualBufferSize) != 0)
            {
                throw new InvalidOperationException("The TotalBufferSize must be divisible by the IndividualBufferSize.");
            }

            // Sets the current index to zero.
            m_CurrentIndex = 0;

            // Sets the size of each individual byte buffer.
            m_IndividualBufferSize = IndividualBufferSize;

            // Initializes a new Stack to hold available buffer space.
            m_FreeIndexPool = new Stack<Int32>();

            // Initializes a new byte buffer.
            m_ByteBuffer = new Byte[TotalBufferSize];
        }

        /// <summary>
        /// Assigns a position in the byte buffer to the SocketAsyncEventArgs event.
        /// </summary>
        /// <param name="SocketEvent">The SocketAsyncEventArgs to assign a portion of the buffer to.</param>
        public void SetBuffer(SocketAsyncEventArgs SocketEvent)
        {
            // Checks if there's available free positions in the byte buffer.
            if (m_FreeIndexPool.Count > 0)
            {
                // Set the position available in the byte buffer to the socket event. At this point, all space
                // in the byte buffer has been assigned, and space is being re-used by new socket events.
                SocketEvent.SetBuffer(m_ByteBuffer, m_FreeIndexPool.Pop(), m_IndividualBufferSize);
            }
            else
            {
                // Check if the current index exceeds the buffer size. This shouldn't happen.
                if ((m_ByteBuffer.Length - m_IndividualBufferSize) < m_CurrentIndex)
                {
                    throw new InvalidOperationException("The pool size of the socket events exceeds the size of the byte buffer.");
                }

                // Set the position available in the byte buffer to the socket event.
                SocketEvent.SetBuffer(m_ByteBuffer, m_CurrentIndex, m_IndividualBufferSize);

                // Increment the position of the current index.
                m_CurrentIndex += m_IndividualBufferSize;
            }
        }

        /// <summary>
        /// Releases the byte buffer from the SocketAsyncEventArgs event.
        /// </summary>
        /// <param name="SocketEvent">The SocketAsyncEventArgs to release the buffer from.</param>
        public void ReleaseBuffer(SocketAsyncEventArgs SocketEvent)
        {
            // Releases the buffer back into the free index pool.
            m_FreeIndexPool.Push(SocketEvent.Offset);

            // Sets the buffer to NULL for the socket event.
            SocketEvent.SetBuffer(null, 0, 0);
        }

        /// <summary>
        /// Gets the total size of the byte buffer.
        /// </summary>
        public Int32 TotalBufferSize
        {
            get
            {
                return m_ByteBuffer.Length;
            }
        }

        /// <summary>
        /// Gets the individual size of each byte buffer.
        /// </summary>
        public Int32 IndividualBufferSize
        {
            get
            {
                return m_IndividualBufferSize;
            }
        }
    }
}
