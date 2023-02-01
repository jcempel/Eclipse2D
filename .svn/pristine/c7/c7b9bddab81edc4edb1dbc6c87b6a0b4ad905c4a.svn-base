using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.RawInput;
using SharpDX.Multimedia;


namespace Eclipse2D.Input
{
    /// <summary>
    /// Represents a mouse device.
    /// </summary>
    public static class Mouse
    {
        /// <summary>
        /// Represents the mouse position relative to the game window.
        /// </summary>
        private static Point m_MousePosition;

        /// <summary>
        /// Represents the mouse wheel's position, in delta.
        /// </summary>
        private static Int32 m_MouseWheelDelta;

        /// <summary>
        /// Represents a collection of buttons currently pressed by the mouse.
        /// </summary>
        private static List<MouseButton> m_MouseButtons;

        /// <summary>
        /// Initializes the mouse.
        /// </summary>
        /// <param name="Window">The game window to attach to the mouse.</param>
        public static void Initialize(GameWindow Window)
        {
            // Initialize the mouse button list.
            m_MouseButtons = new List<MouseButton>(5);

            // Register the mouse with RawInput, and get mouse events directed at the target window.
            Device.RegisterDevice(UsagePage.Generic, UsageId.GenericMouse, DeviceFlags.None, Window.RenderWindow.Handle);

            // Hook the mouse input event.
            Device.MouseInput += Device_MouseInput;
        }

        /// <summary>
        /// Event: Handles mouse input events associated with the underlying render window.
        /// </summary>
        private static void Device_MouseInput(Object Sender, MouseInputEventArgs MouseInput)
        {
            // Get the window from the handle. 
            Control Window = Control.FromHandle(MouseInput.WindowHandle);

            // Checks if the window has focus.
            if (Window != null && Window.Focused)
            {
                // Gets the mouse position relative to the window.
                // TODO: Figure out a better way to handle this through RawInput.
                m_MousePosition = Window.PointToClient(Control.MousePosition);

                // Check if a button was pressed or if the event was just for mouse movement.
                if (MouseInput.ButtonFlags != MouseButtonFlags.None)
                {
                    // Checks if the left mouse button is pressed.
                    if ((MouseInput.ButtonFlags & MouseButtonFlags.LeftButtonDown) == MouseButtonFlags.LeftButtonDown)
                        m_MouseButtons.Add(MouseButton.LeftButton);
                    else if ((MouseInput.ButtonFlags & MouseButtonFlags.LeftButtonUp) == MouseButtonFlags.LeftButtonUp)
                        m_MouseButtons.Remove(MouseButton.LeftButton);

                    // Checks if the right mouse button is pressed.
                    if ((MouseInput.ButtonFlags & MouseButtonFlags.RightButtonDown) == MouseButtonFlags.RightButtonDown)
                        m_MouseButtons.Add(MouseButton.RightButton);
                    else if ((MouseInput.ButtonFlags & MouseButtonFlags.RightButtonUp) == MouseButtonFlags.RightButtonUp)
                        m_MouseButtons.Remove(MouseButton.RightButton);

                    // Checks if the middle mouse button is pressed.
                    if ((MouseInput.ButtonFlags & MouseButtonFlags.MiddleButtonDown) == MouseButtonFlags.MiddleButtonDown)
                        m_MouseButtons.Add(MouseButton.MiddleButton);
                    else if ((MouseInput.ButtonFlags & MouseButtonFlags.MiddleButtonUp) == MouseButtonFlags.MiddleButtonUp)
                        m_MouseButtons.Remove(MouseButton.MiddleButton);

                    // Checks if the additional mouse button 4 is pressed.
                    if ((MouseInput.ButtonFlags & MouseButtonFlags.Button4Down) == MouseButtonFlags.Button4Down)
                        m_MouseButtons.Add(MouseButton.Button4);
                    else if ((MouseInput.ButtonFlags & MouseButtonFlags.Button4Up) == MouseButtonFlags.Button4Up)
                        m_MouseButtons.Remove(MouseButton.Button4);

                    // Checks if the additional mouse button 5 is pressed.
                    if ((MouseInput.ButtonFlags & MouseButtonFlags.Button5Down) == MouseButtonFlags.Button5Down)
                        m_MouseButtons.Add(MouseButton.Button5);
                    else if ((MouseInput.ButtonFlags & MouseButtonFlags.Button5Up) == MouseButtonFlags.Button5Up)
                        m_MouseButtons.Remove(MouseButton.Button5);
                }
                
                // Get the mouse wheel spin delta.
                m_MouseWheelDelta = MouseInput.WheelDelta;
            }
        }

        /// <summary>
        /// Gets the current mouse state.
        /// </summary>
        /// <returns></returns>
        public static MouseState GetState()
        {
            return new MouseState(m_MousePosition, m_MouseWheelDelta, m_MouseButtons);
        }
    }
}
