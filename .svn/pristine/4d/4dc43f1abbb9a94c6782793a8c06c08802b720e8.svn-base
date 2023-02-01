using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eclipse2D.Graphics
{
    public class GraphicsAdapter
    {
        private static List<GraphicsAdapter> m_AdapterList;

        public GraphicsAdapter()
        {

        }

        /// <summary>
        /// Initializes and gathers information for each connected device and output.
        /// </summary>
        public void Initialize()
        {
            // Initialize a new DXGI factory.
            SharpDX.DXGI.Factory1 Factory = new SharpDX.DXGI.Factory1();

            // Initialize the graphics adapter list.
            m_AdapterList = new List<GraphicsAdapter>();

            // Get the adapter count so we can create a list.
            Int32 DeviceCount = Factory.GetAdapterCount();

            // Loop through each device (graphics device) and gather information.
            for (Int32 DeviceIndex = 0; DeviceIndex < DeviceCount; DeviceIndex++)
            {
                // Get the device at the specified index.
                SharpDX.DXGI.Adapter1 Device = Factory.GetAdapter1(DeviceIndex);

                // Get the amount of outputs attached to this device.
                Int32 OutputCount = Device.GetOutputCount();

                // Loop through each output (monitor) and gather information.
                for (Int32 OutputIndex = 0; OutputIndex < OutputCount; OutputCount++)
                {
                    // Get the output at the specified index.
                    SharpDX.DXGI.Output Monitor = Device.GetOutput(OutputIndex);

                    // Initialize the adapter for this device and monitor.
                    GraphicsAdapter Adapter = InitializeAdapter(Device, Monitor);

                    // Add it to the list of adapters.
                    m_AdapterList.Add(Adapter);

                    // Dispose the output.
                    Monitor.Dispose();
                }

                // Dispose the device.
                Device.Dispose();
            }

            // Dispose the factory.
            Factory.Dispose();
        }

        private GraphicsAdapter InitializeAdapter(SharpDX.DXGI.Adapter1 Adapter, SharpDX.DXGI.Output Monitor)
        {
            return null;
        }
    }
}
