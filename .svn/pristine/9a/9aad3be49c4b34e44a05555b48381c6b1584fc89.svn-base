using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eclipse2D
{
    /// <summary>
    /// Represents a collection of services provided to the game.
    /// </summary>
    public class GameServices
    {
        /// <summary>
        /// Represents a collection of services provided to the game.
        /// </summary>
        private Dictionary<Type, Object> m_GameServices;

        /// <summary>
        /// Provides multi-threading support for the service collection.
        /// </summary>
        private readonly Object ServicesLock = new Object();

        /// <summary>
        /// Initializes a new instance of GameServices.
        /// </summary>
        public GameServices()
        {
            m_GameServices = new Dictionary<Type, Object>();
        }

        /// <summary>
        /// Registers the specified type with the specified service provider.
        /// </summary>
        /// <param name="ServiceType">The type of service to register.</param>
        /// <param name="ServiceProvider">The service provider to associate with the type.</param>
        public void RegisterService(Type ServiceType, Object ServiceProvider)
        {
            // Check if the service type is valid.
            if (ServiceType == null)
                throw new ArgumentNullException("ServiceType");

            // Check if the service provider is valid.
            if (ServiceProvider == null)
                throw new ArgumentNullException("ServiceProvider");

            // Check if the service type is assignable from the service provider.
            if (!ServiceType.IsAssignableFrom(ServiceProvider.GetType()))
                throw new ArgumentException(String.Format("Invalid type requested for service provider '{0}'.", ServiceProvider.GetType().FullName));

            lock (ServicesLock)
            {
                // Check if the service collection already contains the specified service.
                if (m_GameServices.ContainsKey(ServiceType))
                    throw new ArgumentException(String.Format("Service type '{0}' already exists.", ServiceType.GetType().FullName));

                // Add the service.
                m_GameServices.Add(ServiceType, ServiceProvider);
            }
        }

        /// <summary>
        /// Registers the specified type with the specified service provider.
        /// </summary>
        /// <typeparam name="T">The type of service to register.</typeparam>
        /// <param name="ServiceProvider">The service provider to associated with the type.</param>
        public void RegisterService<T>(T ServiceProvider)
        {
            RegisterService(typeof(T), ServiceProvider);
        }

        /// <summary>
        /// Un-registers the specified type from the service provider.
        /// </summary>
        /// <param name="ServiceType">The type of service to un-register.</param>
        public void UnregisterService(Type ServiceType)
        {
            // Check if the service type is valid.
            if (ServiceType == null)
                throw new ArgumentNullException("ServiceType");

            lock (ServicesLock)
            {
                // Check if the service collection contains the specified service.
                if (!m_GameServices.ContainsKey(ServiceType))
                    throw new ArgumentException(String.Format("Service type '{0}' doesn't exist.", ServiceType.GetType().FullName));

                // Remove the service.
                m_GameServices.Remove(ServiceType); 
            }
        }

        /// <summary>
        /// Un-registers the specified type from the service provider.
        /// </summary>
        /// <typeparam name="T">The type of service to un-register.</typeparam>
        public void UnregisterService<T>()
        {
            UnregisterService(typeof(T));
        }

        /// <summary>
        /// Gets the service provider for the specified type.
        /// </summary>
        /// <param name="ServiceType">The type of service to get.</param>
        /// <returns>The service provider.</returns>
        public Object GetService(Type ServiceType)
        {
            // Check if the service type is valid.
            if (ServiceType == null)
                throw new ArgumentNullException("ServiceType");

            lock (ServicesLock)
            {
                // Get the specified service.
                if (m_GameServices.ContainsKey(ServiceType))
                    return m_GameServices[ServiceType];
            }

            return null;
        }

        /// <summary>
        /// Gets the service provider for the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to get.</typeparam>
        /// <returns>The service provider.</returns>
        public T GetService<T>()
        {
            // Get the specified service.
            Object Service = GetService(typeof(T));

            // Check if the service is valid.
            if (Service == null)
                throw new ArgumentException(String.Format("Service type '{0}' doesn't exist.", typeof(T).FullName));

            return (T)Service;
        }

        /// <summary>
        /// Gets the amount of game services in the collection.
        /// </summary>
        public Int32 Count
        {
            get
            {
                return m_GameServices.Count;
            }
        }
    }
}
