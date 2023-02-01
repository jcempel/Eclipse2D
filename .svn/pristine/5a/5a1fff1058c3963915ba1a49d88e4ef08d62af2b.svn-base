using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.RawInput;

namespace Eclipse2D.Input
{
    /// <summary>
    /// Represents a mouse state.
    /// </summary>
    public struct MouseState
    {
        /// <summary>
        /// Represents the mouse position relative to the game window.
        /// </summary>
        private Point m_MousePosition;

        /// <summary>
        /// Represents the mouse wheel spins, in delta.
        /// </summary>
        private Int32 m_MouseWheelDelta;

        /// <summary>
        /// Represents a collection of buttons currently pressed by the mouse.
        /// </summary>
        private List<MouseButton> m_MouseButtons;

        /// <summary>
        /// Initializes a new MouseState that is empty.
        /// </summary>
        /// <param name="DefaultSize">The default size of the mouse state collection.</param>
        public MouseState(Int32 DefaultSize)
        {
            m_MousePosition = Point.Empty;
            m_MouseWheelDelta = 0;
            m_MouseButtons = new List<MouseButton>(DefaultSize);
        }

        /// <summary>
        /// Initializes a new MouseState with the specified mouse position, mouse delta, and existing mouse button list.
        /// </summary>
        /// <param name="MousePosition">The current position of the mouse.</param>
        /// <param name="MouseDelta">The current delta of the mouse wheel.</param>
        /// <param name="MouseButtons">The current buttons being pressed on the mouse.</param>
        public MouseState(Point MousePosition, Int32 MouseDelta, List<MouseButton> MouseButtons)
        {
            m_MousePosition = MousePosition;
            m_MouseWheelDelta = MouseDelta;
            m_MouseButtons = new List<MouseButton>(MouseButtons);
        }

        /// <summary>
        /// Checks if a mouse button is pressed.
        /// </summary>
        /// <param name="Button">The mouse button to check.</param>
        /// <returns></returns>
        public Boolean IsButtonPressed(MouseButton Button)
        {
            return m_MouseButtons.Contains(Button);
        }

        /// <summary>
        /// Checks if multiple mouse buttons are pressed.
        /// </summary>
        /// <param name="Button">The mouse button to check.</param>
        /// <param name="ExtraButtons">Additional mouse buttons to check.</param>
        /// <returns></returns>
        public Boolean IsButtonPressed(MouseButton Button, params MouseButtonFlags[] ExtraButtons)
        {
            // Check if the first button is pressed. Writing it like this forces the params keyword
            // to accept at least one parameter, otherwise no parameters would cause issues.
            if (!IsButtonPressed(Button))
            {
                return false;
            }

            // Loop through each additional button.
            foreach (MouseButton ExtraButton in ExtraButtons)
            {
                // Check if the additional button is pressed.
                if (!IsButtonPressed(ExtraButton))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if a mouse button is released.
        /// </summary>
        /// <param name="Button">The mouse button to check.</param>
        /// <returns></returns>
        public Boolean IsButtonReleased(MouseButton Button)
        {
            return !m_MouseButtons.Contains(Button);
        }

        /// <summary>
        /// Checks if multiple mouse buttons are released.
        /// </summary>
        /// <param name="Button">The mouse button to check.</param>
        /// <param name="ExtraButtons">Additional mouse buttons to check.</param>
        /// <returns></returns>
        public Boolean IsButtonReleased(MouseButton Button, params MouseButtonFlags[] ExtraButtons)
        {
            // Check if the first button is released. Writing it like this forces the params keyword
            // to accept at least one parameter, otherwise no parameters would cause issues.
            if (!IsButtonReleased(Button))
            {
                return false;
            }

            // Loop through each additional button.
            foreach (MouseButton ExtraButton in ExtraButtons)
            {
                // Check if the additional button is released.
                if (!IsButtonReleased(ExtraButton))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the current mouse position.
        /// </summary>
        public Point Position
        {
            get
            {
                return m_MousePosition;
            }
        }

        /// <summary>
        /// Gets the current amount of spin applied to the mouse wheel.
        /// </summary>
        public Int32 WheelDelta
        {
            get
            {
                return m_MouseWheelDelta;
            }
        }
    }
}
