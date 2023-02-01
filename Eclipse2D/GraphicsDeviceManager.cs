using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Direct3D;
using Eclipse2D.Graphics;

namespace Eclipse2D
{
    /// <summary>
    /// Represents the graphics device manager which controls the graphics device.
    /// </summary>
    public class GraphicsDeviceManager : IDisposable
    {
        /// <summary>
        /// Represents the game attached to this graphics device manager.
        /// </summary>
        private Game m_Game;

        /// <summary>
        /// Represents the graphics device attached to this graphics device manager.
        /// </summary>
        private GraphicsDevice m_GraphicsDevice;

        /// <summary>
        /// Represents if a draw sequence has been started and is currently pending finish.
        /// </summary>
        private Boolean m_DrawSequenceStarted;

        /// <summary>
        /// Represents if the graphics device is windowed or fullscreen mode.
        /// </summary>
        private Boolean m_IsFullscreen;

        /// <summary>
        /// Represents the width and height of the target resolution.
        /// </summary>
        private Size m_TargetResolution;

        /// <summary>
        /// Represents if the graphics device is syncronizing with the vertical retrace.
        /// </summary>
        private Boolean m_SyncWithVerticalTrace;

        /// <summary>
        /// Represents if the graphics device needs to be restarted.
        /// </summary>
        private Boolean m_DeviceRequiresRestart;

        /// <summary>
        /// Initializes a new graphics device manager with the specified game.
        /// </summary>
        /// <param name="Game"></param>
        public GraphicsDeviceManager(Game Game)
        {
            // Set the game.
            m_Game = Game;
        }

        /// <summary>
        /// Begins the drawing sequence for the render target.
        /// </summary>
        public void BeginDraw()
        {
            // 1. Check for EndDraw exit.
            // 2. Check for device changes.
            // 3. Begin the drawing sequence.
        }

        /// <summary>
        /// Ends the drawing sequence for the render target.
        /// </summary>
        public void EndDraw()
        {
            // 1. Check for BeginDraw entry.
            // 2. End the drawing sequence.
            // 3. Check for errors during the drawing sequence.
            // 4. Recover the device if necessary.
            // 5. Present the drawing sequence.
        }

        /// <summary>
        /// Sets whether the graphics device is running in windowed-mode or full-screen.
        /// </summary>
        /// <param name="IsFullscreen">Determines if the graphics device is running in windowed-mode or full-screen.</param>
        public void SetFullscreen(Boolean IsFullscreen)
        {
            // 1. Set the fullscreen state.
            // 2. The device won't require a restart.
            // 3. Some objects may need to be recreated.
        }

        /// <summary>
        /// Sets the resolution the graphics device is targetting.
        /// </summary>
        /// <param name="Width">The width of the targeted resolution.</param>
        /// <param name="Height">The height of the targeted resolution.</param>
        public void SetResolution(Int32 Width, Int32 Height)
        {
            // 1. Set the fullscreen state.
            // 2. The device won't require a restart.
            // 3. Some objects may need to be recreated.
        }

        public void SetVerticalTrace(Boolean IsVSync)
        {

        }

        public void Dispose()
        {

        }
    }

}
