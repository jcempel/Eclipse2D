using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Windows;

namespace Eclipse2D
{
    /// <summary>
    /// Represents the game window used by the game.
    /// </summary>
    public class GameWindow
    {
        /// <summary>
        /// Represents the underlying render form.
        /// </summary>
        private RenderForm m_RenderForm;

        /// <summary>
        /// Represents the underlying render loop.
        /// </summary>
        private RenderLoop m_RenderLoop;

        /// <summary>
        /// Represents if the game window is running.
        /// </summary>
        private Boolean m_IsRunning;

        /// <summary>
        /// An event that fires when the game window is activated.
        /// </summary>
        public event EventHandler<EventArgs> OnActivated;

        /// <summary>
        /// And event that fires when the game window is de-activated.
        /// </summary>
        public event EventHandler<EventArgs> OnDeactivated;

        /// <summary>
        /// An event that fires when the game window has resumed rendering.
        /// </summary>
        public event EventHandler<EventArgs> OnResumeRendering;

        /// <summary>
        /// An event that fires when the game window has paused rendering.
        /// </summary>
        public event EventHandler<EventArgs> OnPauseRendering;

        /// <summary>
        /// An event that fires when the system is resumed.
        /// </summary>
        public event EventHandler<EventArgs> OnSystemResume;

        /// <summary>
        /// An event that fires when the system is suspended.
        /// </summary>
        public event EventHandler<EventArgs> OnSystemSuspend;

        /// <summary>
        /// An event that fires when the mouse enters the game window.
        /// </summary>
        public event EventHandler<EventArgs> OnMouseEnter;

        /// <summary>
        /// An event that fires when the mouse leaves the game window.
        /// </summary>
        public event EventHandler<EventArgs> OnMouseLeave;

        /// <summary>
        /// Represents the initiate callback method.
        /// </summary>
        public Action InitCallback;

        /// <summary>
        /// Represents the frame callback method.
        /// </summary>
        public Action FrameCallback;

        /// <summary>
        /// Represents the exit callback method.
        /// </summary>
        public Action ExitCallback;

        /// <summary>
        /// Initializes a new game window with the specified title, width, and height.
        /// </summary>
        /// <param name="Title">The title of the game window.</param>
        /// <param name="Width">The width of the client-area for the game window.</param>
        /// <param name="Height">The height of the client-area for the game window.</param>
        public GameWindow(String Title, Int32 Width, Int32 Height)
        {
            // Initialize a new render form.
            m_RenderForm = new RenderForm(Title);

            // Set the size of the client-area.
            m_RenderForm.ClientSize = new Size(Width, Height);

            // Hook important events.
            m_RenderForm.AppActivated += GameWindow_AppActivated;
            m_RenderForm.AppDeactivated += GameWindow_AppDeactivated;
            m_RenderForm.ResumeRendering += GameWindow_ResumeRendering;
            m_RenderForm.PauseRendering += GameWindow_PauseRendering;
            m_RenderForm.SystemResume += GameWindow_SystemResume;
            m_RenderForm.SystemSuspend += GameWindow_SystemSuspend;
            m_RenderForm.MouseEnter += GameWindow_MouseEnter;
            m_RenderForm.MouseLeave += GameWindow_MouseLeave;
        }

        /// <summary>
        /// Runs the window messaging pump.
        /// </summary>
        public void Run()
        {
            // Check that the initialize callback has a valid callback.
            if (InitCallback == null)
            {
                throw new InvalidOperationException("The initialize callback cannot be NULL.");
            }

            // Check that the frame callback has a valid callback.
            if (FrameCallback == null)
            {
                throw new InvalidOperationException("The frame callback cannot be NULL.");
            }

            // Check that the exit callback has a valid callback.
            if (ExitCallback == null)
            {
                throw new InvalidOperationException("The exit callback cannot be NULL.");
            }

            // Call the initialization callback.
            InitCallback();

            try
            {
                // Set the state to running.
                m_IsRunning = true;

                // Show the game window.
                m_RenderForm.Show();

                // Initializes the render loop, which internally uses a light-weight version of DoEvents().
                using (m_RenderLoop = new RenderLoop(m_RenderForm))
                {
                    // Loops continuously until the game is shutdown and processes operating system messages. 
                    while (m_RenderLoop.NextFrame() && m_IsRunning)
                    {
                        // Call the frame callback.
                        FrameCallback();
                    }
                }
            }
            finally
            {
                // Call the exit callback.
                ExitCallback();
            }
        }

        /// <summary>
        /// Sets/Gets the game window title.
        /// </summary>
        public String Title
        {
            set
            {
                m_RenderForm.Text = value;
            }

            get
            {
                return m_RenderForm.Text;
            }
        }

        /// <summary>
        /// Sets/Gets the game window client-area.
        /// </summary>
        public Size ClientSize
        {
            set
            {
                m_RenderForm.ClientSize = value;
            }

            get
            {
                return m_RenderForm.ClientSize;
            }
        }

        /// <summary>
        /// Sets/Gets whether the game window is windowed or full-screen.
        /// </summary>
        public Boolean IsFullscreen
        {
            set
            {
                m_RenderForm.IsFullscreen = value;
            }

            get
            {
                return m_RenderForm.IsFullscreen;
            }
        }

        /// <summary>
        /// Sets/Gets if the game window is currently running.
        /// </summary>
        public Boolean IsRunning
        {
            set
            {
                m_IsRunning = value;
            }

            get
            {
                return m_IsRunning;
            }
        }

        /// <summary>
        /// Event: Executes when the game window is activated.
        /// </summary>
        /// <param name="Sender">The object that created the event.</param>
        /// <param name="Args">The arguments passed by this event.</param>
        private void GameWindow_AppActivated(Object Sender, EventArgs Args)
        {
            OnActivated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event: Executes when the game window is de-activated.
        /// </summary>
        /// <param name="Sender">The object that created the event.</param>
        /// <param name="Args">The arguments passed by this event.</param>
        private void GameWindow_AppDeactivated(Object Sender, EventArgs Args)
        {
            OnDeactivated?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event: Executes when the game window has resumed rendering.
        /// </summary>
        /// <param name="Sender">The object that created the event.</param>
        /// <param name="Args">The arguments passed by this event.</param>
        private void GameWindow_ResumeRendering(Object Sender, EventArgs Args)
        {
            OnResumeRendering?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event: Executes when the game window has paused rendering.
        /// </summary>
        /// <param name="Sender">The object that created the event.</param>
        /// <param name="Args">The arguments passed by this event.</param>
        private void GameWindow_PauseRendering(Object Sender, EventArgs Args)
        {
            OnPauseRendering?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event: Executes when the system is resumed.
        /// </summary>
        /// <param name="Sender">The object that created the event.</param>
        /// <param name="Args">The arguments passed by this event.</param>
        private void GameWindow_SystemResume(Object Sender, EventArgs Args)
        {
            OnSystemResume?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event: Executes when the system is suspended.
        /// </summary>
        /// <param name="Sender">The object that created the event.</param>
        /// <param name="Args">The arguments passed by this event.</param>
        private void GameWindow_SystemSuspend(Object Sender, EventArgs Args)
        {
            OnSystemSuspend?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event: Executes when the mouse enters the game window.
        /// </summary>
        /// <param name="Sender">The object that created the event.</param>
        /// <param name="Args">The arguments passed by this event.</param>
        private void GameWindow_MouseEnter(Object Sender, EventArgs Args)
        {
            OnMouseEnter?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Event: Executes when the game window leaves the game window.
        /// </summary>
        /// <param name="Sender">The object that created the event.</param>
        /// <param name="Args">The arguments passed by this event.</param>
        private void GameWindow_MouseLeave(Object Sender, EventArgs Args)
        {
            OnMouseLeave?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Gets the handle for the underlying render form.
        /// </summary>
        public RenderForm RenderWindow
        {
            get
            {
                return m_RenderForm;
            }
        }
    }
}
