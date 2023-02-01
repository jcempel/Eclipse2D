using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eclipse2D.Input
{
    /// <summary>
    /// Represents a keyboard state.
    /// </summary>
    public struct KeyboardState
    {
        /// <summary>
        /// Represents a collection of keys being pressed.
        /// </summary>
        private List<Keys> m_KeyboardKeys;

        /// <summary>
        /// Initializes a new KeyboardState that is empty.
        /// </summary>
        /// <param name="DefaultSize">The default size of the keyboard state collection.</param>
        public KeyboardState(Int32 DefaultSize)
        {
            // Initializes the list.
            m_KeyboardKeys = new List<Keys>(DefaultSize);
        }

        /// <summary>
        /// Initializes a new KeyboardState from an existing key collection.
        /// </summary>
        /// <param name="ExistingKeys">The list of keys to copy into the keyboard state.</param>
        public KeyboardState(List<Keys> ExistingKeys)
        {
            // Initializes the list.
            m_KeyboardKeys = new List<Keys>(ExistingKeys);
        }

        /// <summary>
        /// Checks if a keyboard key is pressed.
        /// </summary>
        /// <param name="Key">The keyboard key to check.</param>
        /// <returns></returns>
        public Boolean IsKeyPressed(Keys Key)
        {
            return m_KeyboardKeys.Contains(Key);
        }

        /// <summary>
        /// Checks if multiple keyboard keys are pressed.
        /// </summary>
        /// <param name="Key">The keyboard key to check.</param>
        /// <param name="ExtraKeys">Additional keyboard keys to check.</param>
        /// <returns></returns>
        public Boolean IsKeyPressed(Keys Key, params Keys[] ExtraKeys)
        {
            // Check if the first key is pressed. Writing it like this forces the params keyword
            // to accept at least one parameter, otherwise no parameters would cause issues.
            if (!IsKeyPressed(Key))
            {
                return false; 
            }

            // Loop through each additional key.
            foreach (Keys ExtraKey in ExtraKeys)
            {
                // Check if the additional key is pressed.
                if (!IsKeyPressed(ExtraKey))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Checks if a keyboard key is released.
        /// </summary>
        /// <param name="Key">The keyboard key to check.</param>
        /// <returns></returns>
        public Boolean IsKeyReleased(Keys Key)
        {
            return !m_KeyboardKeys.Contains(Key);
        }

        /// <summary>
        /// Checks if multiple keyboard keys are released.
        /// </summary>
        /// <param name="Key">The keyboard key to check.</param>
        /// <param name="ExtraKeys">Additional keyboard keys to check.</param>
        /// <returns></returns>
        public Boolean IsKeyReleased(Keys Key, params Keys[] ExtraKeys)
        {
            // Check if the first key is released. Writing it like this forces the params keyword
            // to accept at least one parameter, otherwise no parameters would cause issues.
            if (!IsKeyReleased(Key))
            {
                return false;
            }

            // Loop through each additional key.
            foreach (Keys ExtraKey in ExtraKeys)
            {
                // Check if the additional key is released.
                if (!IsKeyReleased(ExtraKey))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sets if the specified key is being pressed or released.
        /// </summary>
        /// <param name="Key">The keyboard key.</param>
        /// <param name="State">The state of the keyboard key. </param>
        /// <remarks>Setting the key state only affects this KeyboardState.</remarks>
        public void SetKeyState(Keys Key, KeyboardButtonState State)
        {
            // Checks the button state.
            if (State == KeyboardButtonState.Pressed)
            {
                // Check if the list already contains the key, if not, add it.
                if (!m_KeyboardKeys.Contains(Key))
                {
                    m_KeyboardKeys.Add(Key);
                }
            }
            else
            {
                // Check if the list contains the key, if so, remove it.
                if (m_KeyboardKeys.Contains(Key))
                {
                    m_KeyboardKeys.Remove(Key);
                }
            }
        }

        /// <summary>
        /// Gets if the specified key is being pressed or released.
        /// </summary>
        /// <param name="Key">The keyboard key.</param>
        /// <returns></returns>
        public KeyboardButtonState GetKeyState(Keys Key)
        {
            return m_KeyboardKeys.Contains(Key) ? KeyboardButtonState.Pressed : KeyboardButtonState.Released;
        }
    }
}
