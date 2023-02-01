using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Eclipse2D.Network.Events;

namespace Eclipse2D.Network
{
    /// <summary>
    /// Represents a network server, which can asynchronously send and receive data.
    /// </summary>
    public class NetworkServer
    {
        /// <summary>
        /// Represents the listening socket.
        /// </summary>
        private Socket m_ListenSocket;

        /// <summary>
        /// Represents a network buffer used with all receive requests.
        /// </summary>
        private NetworkBufferManager m_ReceiveBufferManager;

        /// <summary>
        /// Represents a network buffer used with all send requests.
        /// </summary>
        private NetworkBufferManager m_SendBufferManager;

        /// <summary>
        /// Represents a pool of socket events used for receiving data.
        /// </summary>
        private SocketAsyncEventArgsPool m_SocketReceiveEventPool;

        /// <summary>
        /// Represents a pool of socket events used for sending data.
        /// </summary>
        private SocketAsyncEventArgsPool m_SocketSendEventPool;

        /// <summary>
        /// Represents a collection of network sessions.
        /// </summary>
        private Dictionary<Int32, NetworkSession> m_NetworkSessions;

        /// <summary>
        /// Represents the individual buffer size for each connected client.
        /// </summary>
        private Int32 m_BufferSize;

        /// <summary>
        /// Represents the maximum amount of connections.
        /// </summary>
        private Int32 m_MaxConnections;

        /// <summary>
        /// Represents the current amount of connections.
        /// </summary>
        private Int32 m_CurrentConnections;

        /// <summary>
        /// Represents the index for the next accepted connection.
        /// </summary>
        private Int32 m_CurrentConnectionIndex;

        /// <summary>
        /// Represents a signal on the connecting clients so we don't exceed the maximum amount of connections.
        /// </summary>
        private SemaphoreSlim m_ConnectSignal;

        /// <summary>
        /// Represents the local end point of the host machine.
        /// </summary>
        private IPEndPoint m_IPEndPoint;

        /// <summary>
        /// Represents if the network server is running.
        /// </summary>
        private Boolean m_IsRunning;

        /// <summary>
        /// Represents if the network server is building partial or complete packets.
        /// </summary>
        private Boolean m_BuildCompletePackets;

        /// <summary>
        /// An event that fires when a new network session has connected.
        /// </summary>
        public event EventHandler<ConnectedEventArgs> OnSessionConnected;

        /// <summary>
        /// An event that fires when an existing network session has disconnected.
        /// </summary>
        public event EventHandler<DisconnectedEventArgs> OnSessionDisconnected;

        /// <summary>
        /// An event that fires when a network session has received data.
        /// </summary>
        public event EventHandler<DataReceivedEventArgs> OnSessionDataReceived;

        /// <summary>
        /// An event that fires when a network session has sent data.
        /// </summary>
        public event EventHandler<DataSentEventArgs> OnSessionDataSent;

        /// <summary>
        /// Initializes a new NetworkServer with the specified maximum amount of connections, individual buffer size, and if the network server is building packets.
        /// </summary>
        /// <param name="MaxConnections">The maximum amount of connections.</param>
        /// <param name="IndividualBufferSize">The individual size of each buffer.</param>
        /// <param name="BuildCompletePackets">Whether or not the network server is building complete packets or using raw data.</param>
        public NetworkServer(Int32 MaxConnections, Int32 IndividualBufferSize, Boolean BuildCompletePackets)
        {
            // Set the maximum amount of connections.
            m_MaxConnections = MaxConnections;

            // Set the individual buffer size.
            m_BufferSize = IndividualBufferSize;

            // Determine if we're building complete packets.
            m_BuildCompletePackets = BuildCompletePackets;

            // Initialize the network sessions collection.
            m_NetworkSessions = new Dictionary<Int32, NetworkSession>(MaxConnections);

            // Initialize the socket receive events.
            InitializeSocketReceiveEvents();

            // Initialize the socket send events.
            InitializeSocketSendEvents();
        }

        /// <summary>
        /// Initializes a group of socket events that receive data.
        /// </summary>
        private void InitializeSocketReceiveEvents()
        {
            // Initializes the buffer manager which manages all of the individual buffers for each socket.
            m_ReceiveBufferManager = new NetworkBufferManager(m_MaxConnections * m_BufferSize, m_BufferSize);

            // Initializes the socket receive event pool.
            m_SocketReceiveEventPool = new SocketAsyncEventArgsPool(m_MaxConnections);

            // The socket receive event that is used to populate the buffer and pool.
            SocketAsyncEventArgs SocketReceiveEvent;

            // Initialize each socket receive event.
            for (Int32 I = 0; I < MaxConnections; I++)
            {
                // Initialize the socket receive event.
                SocketReceiveEvent = new SocketAsyncEventArgs();

                // Hook the Completed event.
                SocketReceiveEvent.Completed += SocketEvent_IOCompleted;

                // Initialize the user token to null.
                SocketReceiveEvent.UserToken = null;

                // Set the buffer space used with this socket receive event.
                m_ReceiveBufferManager.SetBuffer(SocketReceiveEvent);

                // Push the socket receive event to the socket pool.
                m_SocketReceiveEventPool.Push(SocketReceiveEvent);
            }
        }

        /// <summary>
        /// Initializes a group of socket events that send data.
        /// </summary>
        private void InitializeSocketSendEvents()
        {
            // Initializes the buffer manager which manages all of the individual buffers for each socket.
            m_SendBufferManager = new NetworkBufferManager(m_MaxConnections * m_BufferSize, m_BufferSize);

            // Initializes the socket send event pool.
            m_SocketSendEventPool = new SocketAsyncEventArgsPool(m_MaxConnections);

            // The socket send event that is used to populate the buffer and pool.
            SocketAsyncEventArgs SocketSendEvent;

            // Initialize each socket send event.
            for (Int32 I = 0; I < MaxConnections; I++)
            {
                // Initialize the socket send event.
                SocketSendEvent = new SocketAsyncEventArgs();

                // Hook the Completed event.
                SocketSendEvent.Completed += SocketEvent_IOCompleted;

                // Initialize the user token to null.
                SocketSendEvent.UserToken = null;

                // Set the buffer space used with this socket send event.
                m_SendBufferManager.SetBuffer(SocketSendEvent);

                // Push the socket send event to the socket pool.
                m_SocketSendEventPool.Push(SocketSendEvent);
            }
        }

        /// <summary>
        /// Begins listening asynchronously with the specified port.
        /// </summary>
        /// <param name="Port">The port to listen on. (1 - 65535, or 0 for auto-assign).</param>
        public void Listen(Int32 Port)
        {
            Listen(IPAddress.IPv6Any, Port);
        }

        /// <summary>
        /// Begins listening asynchronously with the specified host name and port.
        /// </summary>
        /// <param name="HostAddress">The host address of the server.</param>
        /// <param name="Port">The port to listen on. (1 - 65535, or 0 for auto-assign).</param>
        public void Listen(IPAddress HostAddress, Int32 Port)
        {
            Listen(HostAddress, Port, 100);
        }

        /// <summary>
        /// Begins listening asynchronously with the specified host name, port, back log, maximum amount of connections, and the size of each individual buffer.
        /// </summary>
        /// <param name="HostAddress">The host address of the network server.</param>
        /// <param name="Port">The port to listen on (valid range is 1 - 65535, or 0 for auto-assign).</param>
        /// <param name="BackLog">The size of the back log.</param>
        public void Listen(IPAddress HostAddress, Int32 Port, Int32 BackLog)
        {
            // Check if the network server is running.
            if (m_IsRunning)
            {
                throw new InvalidOperationException("Attempted to start the network server when it has already been initialized.");
            }

            // Check if the port is using a valid range.
            if (Port < 0 || Port > 65535)
            {
                throw new InvalidOperationException("The port range specified is invalid. Valid port range is from 0 to 65535.");
            }

            // Initialize the connect semaphore which blocks the accept thread from exceeding
            // the maximum amount of connections.
            m_ConnectSignal = new SemaphoreSlim(MaxConnections, MaxConnections);

            // Set the next connection index to zero.
            m_CurrentConnectionIndex = 0;

            try
            {
                // Initialize the IP end point for the local host.
                m_IPEndPoint = new IPEndPoint(HostAddress, Port);

                // Initializes an IPv4 or IPv6 TCP socket. By default, the frame work uses an IPv6
                // socket with dual mode enabled. This allows both IPv4 and IPv6 connections to connect
                // using only one listening socket.
                m_ListenSocket = new Socket(HostAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                
                // Check if the address family is IPv6.
                if (HostAddress.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    // Enable dual mode to allow both IPv4 and IPv6 connections.
                    m_ListenSocket.SetSocketOption(SocketOptionLevel.IPv6, SocketOptionName.IPv6Only, 0);
                }

                // Set the server state to running.
                m_IsRunning = true;

                // Bind to the requested port.
                m_ListenSocket.Bind(m_IPEndPoint);

                // Start listening on the requested port.
                m_ListenSocket.Listen(BackLog);

            }
            catch (Exception)
            {
                throw new Exception("Failed to initialize the network server.");
            }

            // Begin listening for new connection requests.
            new Thread(new ThreadStart(() => Server_BeginAccept(null))).Start();
        }

        /// <summary>
        /// Closes the network server.
        /// </summary>
        public void Close()
        {
            // Check if the network server is running.
            if (!m_IsRunning)
            {
                throw new InvalidOperationException("Attempted to stop the network server when it hasn't been initialized.");
            }

            // Set the server state to stopped.
            m_IsRunning = false;

            // Shutdown and dispose the listening socket.
            m_ListenSocket.Dispose();

            // Close all network sessions.
            foreach (var Session in m_NetworkSessions)
            {
                // Shutdown and dispose the socket.
                DisposeSocket(Session.Value.Socket);

                // Let any listeners know that the socket has disconnected.
                OnSessionDisconnected?.Invoke(this, new DisconnectedEventArgs());
            }

            // Clear the network sessions.
            m_NetworkSessions.Clear();

            // Closes the semaphore.
            m_ConnectSignal.Dispose();

            // Set the current amount of connected clients.
            m_CurrentConnections = 0;
        }

        /// <summary>
        /// Closes the receive socket event.
        /// </summary>
        /// <param name="SocketEvent">The socket event that caused the close.</param>
        /// <param name="Reason">The reason the socket was forced to close.</param>
        private void CloseReceiveSession(SocketAsyncEventArgs SocketEvent, SocketCloseReason Reason)
        {
            // Get the network session from the user token.
            NetworkSession Session = SocketEvent.UserToken as NetworkSession;

            // Check if the session exists.
            if (Session != null)
            {
                // Shutdown and dispose the socket.
                DisposeSocket(Session.Socket);

                // Remove the network session from the collection.
                m_NetworkSessions.Remove(Session.SessionIndex);

                // Set the user token to null.
                SocketEvent.UserToken = null;

                // Let any listeners know that the socket has disconnected.
                OnSessionDisconnected?.Invoke(this, new DisconnectedEventArgs());
            }

            // Push the socket event back into the socket pool.
            m_SocketReceiveEventPool.Push(SocketEvent);

            // Decrement the current amount of connections.
            Interlocked.Decrement(ref m_CurrentConnections);

            // Release a client from the semaphore.
            m_ConnectSignal.Release();
        }

        /// <summary>
        /// Closes the send socket event.
        /// </summary>
        /// <param name="SocketEvent">The socket event that caused the close.</param>
        /// <param name="Reason">The reason the socket was forced to close.</param>
        private void CloseSendSession(SocketAsyncEventArgs SocketEvent, SocketCloseReason Reason)
        {
            // Set the user token to null.
            SocketEvent.UserToken = null;

            // Push the socket event back into the socket pool.
            m_SocketSendEventPool.Push(SocketEvent);
        }

        /// <summary>
        /// Begins accepting a new asynchronous connection.
        /// </summary>
        /// <param name="AcceptEvent">The socket event being re-used for each new connection.</param>
        private void Server_BeginAccept(SocketAsyncEventArgs AcceptEvent)
        {
            // Check if the network server is still running.
            if (m_IsRunning)
            {
                // Checks if the accept event is NULL.
                if (AcceptEvent == null)
                {
                    // Initialize the accept event. This only occurs one time and is re-used
                    // for each new incoming connection.
                    AcceptEvent = new SocketAsyncEventArgs();

                    // Hook the Completed event for when an accept requires an asynchronous operation. 
                    AcceptEvent.Completed += SocketEvent_IOCompleted;
                }
                else
                {
                    // The accept event has already been initialized, but the AcceptSocket must
                    // be set to NULL so we can avoid referencing it in the future.
                    AcceptEvent.AcceptSocket = null;
                }

                // Waits until a network session is free for use. This prevents the network server
                // from going over the maximum amount of connected clients.
                m_ConnectSignal.Wait();

                // Begin an asynchronous call to accept the next incoming connection. Sometimes
                // the incoming connection is already waiting, so we handle it right away.
                if (!m_ListenSocket.AcceptAsync(AcceptEvent))
                {
                    // Handle the new connection immediately.
                    Server_EndAccept(AcceptEvent);
                }
            }
        }

        /// <summary>
        /// Ends accepting a new asynchronous connection.
        /// </summary>
        /// <param name="AcceptEvent"></param>
        private void Server_EndAccept(SocketAsyncEventArgs AcceptEvent)
        {
            // Check if the network server is still running.
            if (m_IsRunning)
            {
                // Check if a socket error occured.
                if (AcceptEvent.SocketError == SocketError.Success)
                {
                    // Increment the amount of connected clients.
                    Interlocked.Increment(ref m_CurrentConnections);

                    // Increment the current connection index.
                    Int32 ConnectionIndex = Interlocked.Increment(ref m_CurrentConnectionIndex);

                    // Pop a socket receive event from the socket pool.
                    SocketAsyncEventArgs SocketReceiveEvent = m_SocketReceiveEventPool.Pop();

                    // Enable no delay for the accepted socket.
                    AcceptEvent.AcceptSocket.SetSocketOption(SocketOptionLevel.Tcp, SocketOptionName.NoDelay, true);

                    // Initiate the network session.
                    NetworkSession Session = new NetworkSession(this, ConnectionIndex, AcceptEvent.AcceptSocket);

                    // Set the network session to the user token.
                    SocketReceiveEvent.UserToken = Session;

                    // Add the network session to the collection.
                    m_NetworkSessions.Add(ConnectionIndex, Session);

                    // Let any listeners know that a new connection has been established.
                    OnSessionConnected?.Invoke(this, new ConnectedEventArgs(AcceptEvent.AcceptSocket));

                    // Begin the first receive.
                    Server_BeginReceive(SocketReceiveEvent);

                    // Accept the next connection request.
                    Server_BeginAccept(AcceptEvent);
                }
                else
                {
                    // Close the network server because an error was encountered.
                    Close();
                }
            }
        }

        /// <summary>
        /// Event: Handles when a call from AcceptAsync, ReceiveAsync, or SendAsync has been completed asynchronously.
        /// </summary>
        /// <param name="Sender">The object that raised the event.</param>
        /// <param name="SocketEvent">The socket even that completed.</param>
        private void SocketEvent_IOCompleted(Object Sender, SocketAsyncEventArgs SocketEvent)
        {
            // Determine if the socket event is an accept, read, or write operation.
            switch (SocketEvent.LastOperation)
            {
                case SocketAsyncOperation.Accept:
                    Server_EndAccept(SocketEvent);
                    break;

                case SocketAsyncOperation.Send:
                    Server_EndSend(SocketEvent);
                    break;

                case SocketAsyncOperation.Receive:
                    Server_EndReceive(SocketEvent);
                    break;

                default:
                    throw new InvalidOperationException("The IO completion received an invalid operation.");
            }
        }

        /// <summary>
        /// Begins receiving data using the specified socket event.
        /// </summary>
        /// <param name="SocketReceiveEvent">The event to use to begin the socket receive.</param>
        private void Server_BeginReceive(SocketAsyncEventArgs SocketReceiveEvent)
        {
            // Check if the network server is still running.
            if (m_IsRunning)
            {
                // Get the network session from the user token.
                NetworkSession Session = SocketReceiveEvent.UserToken as NetworkSession;

                // Check that a valid network session exists.
                if (Session != null)
                {
                    // Begin receiving data asynchronously on this socket receive event.
                    if (!Session.Socket.ReceiveAsync(SocketReceiveEvent))
                    {
                        // End the receive if data is already available, and process it.
                        Server_EndReceive(SocketReceiveEvent);
                    }
                }
            }
        }

        /// <summary>
        /// Ends a receive request for the specified socket event.
        /// </summary>
        /// <param name="SocketEvent">The event to use to end the socket receive.</param>
        private void Server_EndReceive(SocketAsyncEventArgs SocketEvent)
        {
            // Check if the network server is still running.
            if (m_IsRunning)
            {
                // Check if the receive operation was successful.
                if (SocketEvent.SocketError == SocketError.Success)
                {
                    // Get the network session from the user token.
                    NetworkSession Session = SocketEvent.UserToken as NetworkSession;

                    // Check if the network session is valid.
                    if (Session == null)
                    {
                        // Close the socket with an error.
                        CloseReceiveSession(SocketEvent, SocketCloseReason.SessionNull);
                        return;
                    }

                    // Check if the network session was closed remotely.
                    if (SocketEvent.BytesTransferred == 0)
                    {
                        // Close the socket successfully.
                        CloseReceiveSession(SocketEvent, SocketCloseReason.RemotelyClosed);
                        return;
                    }

                    // The receive buffer for the completed byte stream.
                    Byte[] ReceiveBuffer;

                    // Check if the network server is building partial or complete packets.
                    if (m_BuildCompletePackets)
                    {
                        // Process and build completed packets. These are packets you can use the PacketReader
                        // and PacketWriter classes with. They're built by the network server so the programmer
                        // can focus on handling the data inside of them.
                        Session.ProcessReceive(SocketEvent);

                        // Handle packets immediately after they're built. Sometimes one receive can build several small
                        // packets, so we handle them in the order they were received. If no packets were built with this
                        // receive process, the loop is skipped entirely.
                        while (Session.DataAvailableForRead)
                        {
                            // De-queue the next completed byte stream.
                            ReceiveBuffer = Session.ReceiveQueue.Dequeue();

                            // Let any listeners know that data has been received.
                            OnSessionDataReceived?.Invoke(this, new DataReceivedEventArgs(Session.Socket, ReceiveBuffer));
                        }
                    }
                    else
                    {
                        // Process the raw data received by the network server. Processing raw data provides no length checks,
                        // no packet processing, etc. This is intended for programmers who want to handle their own data, or have
                        // a special need or requirement with the way their packets are built.
                        Session.ProcessReceiveRaw(SocketEvent);

                        // De-queue the next raw byte stream.
                        ReceiveBuffer = Session.ReceiveQueue.Dequeue();

                        // Let any listeners know that data has been received.
                        OnSessionDataReceived?.Invoke(this, new DataReceivedEventArgs(Session.Socket, ReceiveBuffer));
                    }

                    // Begin the next receive.
                    Server_BeginReceive(SocketEvent);
                }
                else
                {
                    // Close the socket with an error.
                    CloseReceiveSession(SocketEvent, SocketCloseReason.SocketError);
                }
            }
        }

        /// <summary>
        /// Sends a byte array to the specified network session.
        /// </summary>
        /// <param name="Session">The network session to perform the send.</param>
        /// <param name="SendData">The byte array to be sent.</param>
        /// <param name="GroupedData">Provide additional byte arrays to group send operations.</param>
        public void SendBytes(NetworkSession Session, Byte[] SendData, params Byte[][] GroupedData)
        {
            // Check that a valid network session exists.
            if (Session != null)
            {
                // Wait for the previous send operation to complete.
                Session.SendSignal.Wait();

                // Enqueue the data pending to be sent.
                Session.SendQueue.Enqueue(SendData);

                // The network server can group packets together to optimize send operations.
                foreach (Byte[] Data in GroupedData)
                {
                    // Enqueue the grouped data pending to be sent.
                    Session.SendQueue.Enqueue(Data);
                }

                // Send the queued data.
                SendInternal(Session);
            }
        }

        /// <summary>
        /// Sends a byte array to the specified network session.
        /// </summary>
        /// <param name="Session">The network session to perform the send.</param>
        private void SendInternal(NetworkSession Session)
        {
            // Check that a valid network session exists.
            if (Session != null)
            {
                // Check that there is data available to send.
                if (Session.DataAvailableForWrite)
                {
                    // Pop the next available send event.
                    SocketAsyncEventArgs SendEvent = m_SocketSendEventPool.Pop();

                    // Attach the network session to the send event.
                    SendEvent.UserToken = Session;

                    // Begin the asynchronous send.
                    Server_BeginSend(SendEvent);
                }
            }
        }

        /// <summary>
        /// Begins sending data using the specified socket event.
        /// </summary>
        /// <param name="SocketEvent">The event to use to begin the socket send.</param>
        private void Server_BeginSend(SocketAsyncEventArgs SocketEvent)
        {
            // Check if the network server is still running.
            if (m_IsRunning)
            {
                // Get the network session from the user token.
                NetworkSession Session = SocketEvent.UserToken as NetworkSession;

                // Check that a valid network session exists.
                if (Session != null)
                {
                    // Prepares the data requesting to be sent.
                    Session.ProcessSend(SocketEvent);

                    // Begin sending data asynchronously on this socket event.
                    if (!Session.Socket.SendAsync(SocketEvent))
                    {
                        // End the send if data is already available, and process it.
                        Server_EndSend(SocketEvent);
                    }
                }
            }
        }

        /// <summary>
        /// Ends a send request for the specified socket event.
        /// </summary>
        /// <param name="SocketEvent">The event to use to end the socket send.</param>
        private void Server_EndSend(SocketAsyncEventArgs SocketEvent)
        {
            // Check if the network server is still running.
            if (m_IsRunning)
            {
                // Check if the send operation was successful.
                if (SocketEvent.SocketError == SocketError.Success)
                {
                    // Get the network session from the user token.
                    NetworkSession Session = SocketEvent.UserToken as NetworkSession;

                    // Check if the network session is valid.
                    if (Session == null)
                    {
                        // Close the socket with an error.
                        CloseSendSession(SocketEvent, SocketCloseReason.SessionNull);
                        return;
                    }

                    // Let any listeners know that data has been sent.
                    OnSessionDataSent?.Invoke(this, new DataSentEventArgs());

                    // Check if more data is available to be sent.
                    if (Session.DataAvailableForWrite)
                    {
                        // Begin sending the rest of the data in the send queue.
                        Server_BeginSend(SocketEvent);
                    }
                    else
                    {
                        // Push the send event back into the socket pool.
                        m_SocketSendEventPool.Push(SocketEvent);

                        // Release the semaphore on the network session.
                        Session.SendSignal.Release(1);
                    }
                }
                else
                {
                    // Close the socket with an error.
                    CloseSendSession(SocketEvent, SocketCloseReason.SocketError);
                }
            }
        }

        /// <summary>
        /// Shuts down receive and send operations, and disposes a socket safely.
        /// </summary>
        /// <param name="UnsafeSocket">The socket to be disposed.</param>
        private void DisposeSocket(Socket UnsafeSocket)
        {
            try
            {
                // Shutdown send and receive operations.
                UnsafeSocket.Shutdown(SocketShutdown.Both);

                // Dispose the socket.
                UnsafeSocket.Dispose();
            }
            catch (Exception)
            {
                // Catch all exceptions and ignore the socket error, if encountered.
            }
            finally
            {
                // Set the socket to null.
                UnsafeSocket = null;
            }
        }

        /// <summary>
        /// Gets the size of buffer assigned to each individual network session.
        /// </summary>
        public Int32 BufferSize
        {
            get
            {
                return m_BufferSize;
            }
        }

        /// <summary>
        /// Gets the maximum amount of connections supported by the network server.
        /// </summary>
        public Int32 MaxConnections
        {
            get
            {
                return m_MaxConnections;
            }
        }

        /// <summary>
        /// Gets the amount of network sessions currently connected to the network server.
        /// </summary>
        public Int32 CurrentConnections
        {
            get
            {
                return m_CurrentConnections;
            }
        }

        /// <summary>
        /// Gets the local end point the network server is bound to.
        /// </summary>
        public IPEndPoint LocalEndPoint
        {
            get
            {
                return m_IPEndPoint;
            }
        }

        /// <summary>
        /// Gets if the network server is currently running.
        /// </summary>
        public Boolean IsRunning
        {
            get
            {
                return m_IsRunning;
            }
        }

        /// <summary>
        /// Gets if the network server is building partial or complete packets.
        /// </summary>
        public Boolean BuildPackets
        {
            get
            {
                return m_BuildCompletePackets;
            }
        }
    }
}
