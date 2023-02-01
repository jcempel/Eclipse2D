using System;
using System.Windows.Forms;
using System.Collections.Generic;
using Microsoft.Win32;
using System.Management;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.RawInput;
using SharpDX.Multimedia;

namespace Eclipse2D.Input
{
    /// <summary>
    /// Represents a keyboard device.
    /// </summary>
    public static class Keyboard
    {
        /// <summary>
        /// Represents a collection of keys currently pressed by the keyboard.
        /// </summary>
        private static List<Keys> m_KeyboardKeys;

        /// <summary>
        /// Initializes the keyboard.
        /// </summary>
        /// <param name="Window">The game window to attach to the keyboard.</param>
        public static void Initialize(GameWindow Window)
        {
            // Initialize the key list.
            m_KeyboardKeys = new List<Keys>(5);

            // Register the keyboard with RawInput, and get keyboard events directed at the target window.
            Device.RegisterDevice(UsagePage.Generic, UsageId.GenericKeyboard, DeviceFlags.None, Window.RenderWindow.Handle);

            // Hook the keyboard input event.
            Device.KeyboardInput += Device_KeyboardInput;
        }

        /// <summary>
        /// Event: Handles keyboard input events associated with the underlying render window.
        /// </summary>
        private static void Device_KeyboardInput(Object Sender, KeyboardInputEventArgs KeyboardInput)
        {
            // Get the window from the handle. 
            Control Window = Control.FromHandle(KeyboardInput.WindowHandle);

            // Check if the window has focus.
            if (Window != null && Window.Focused)
            {
                // Checks if the key is down by the user or system.
                if (KeyboardInput.State == KeyState.KeyDown || KeyboardInput.State == KeyState.SystemKeyDown)
                {
                    // Checks if the list already contains this key. We only want to add the key once,
                    // because if the key still exists, then the key is being held down.
                    if (!m_KeyboardKeys.Contains(KeyboardInput.Key))
                    {
                        // Add the key to the key list.
                        m_KeyboardKeys.Add(KeyboardInput.Key);
                    }
                }

                // Checks if the key is up by the user or system.
                if (KeyboardInput.State == KeyState.KeyUp || KeyboardInput.State == KeyState.SystemKeyUp)
                {
                    // Checks if the list contains this key.
                    if (m_KeyboardKeys.Contains(KeyboardInput.Key))
                    {
                        // Remove the key from the key list.
                        m_KeyboardKeys.Remove(KeyboardInput.Key);
                    }
                }
            }
            else
            {
                // Clear the list if the window has lost focus.
                m_KeyboardKeys.Clear();
            }
        }

        /// <summary>
        /// Gets the current keyboard state.
        /// </summary>
        /// <returns></returns>
        public static KeyboardState GetState()
        {
            return new KeyboardState(m_KeyboardKeys);
        }
    }
}
