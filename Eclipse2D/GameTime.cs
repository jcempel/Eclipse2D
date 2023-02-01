using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eclipse2D
{
    /// <summary>
    /// Represents a game timer which operates in delta-time.
    /// </summary>
    public class GameTime
    {
        /// <summary>
        /// Represents the amount of seconds per count.
        /// </summary>
        private readonly Double m_SecondsPerCount;

        /// <summary>
        /// Represents the amount of time since the last frame, in delta-time.
        /// </summary>
        private Double m_DeltaTime;

        /// <summary>
        /// Represents the game time of the previous frame.
        /// </summary>
        private Int64 m_PreviousTime;

        /// <summary>
        /// Represents the game time of the current frame.
        /// </summary>
        private Int64 m_CurrentTime;

        /// <summary>
        /// Initializes a new GameTime.
        /// </summary>
        public GameTime()
        {
            // Initialize variables.
            m_DeltaTime = 0.0F;
            m_PreviousTime = 0;
            m_CurrentTime = 0;

            m_SecondsPerCount = 1.0 / Stopwatch.Frequency;
        }

        /// <summary>
        /// Updates the elapsed time, in delta, from the previous tick.
        /// </summary>
        public void Tick()
        {
            // Get the latest timestamp.
            m_CurrentTime = Stopwatch.GetTimestamp();

            // Update the time it took to complete the previous frame.
            m_DeltaTime = (m_CurrentTime - m_PreviousTime) * m_SecondsPerCount;

            // The current time is now the previous time, and this will be used to compute
            // the time it took to complete the previous frame the next time we update the frame.
            m_PreviousTime = m_CurrentTime;
        }

        /// <summary>
        /// Gets the time it took to complete the last frame.
        /// </summary>
        public Double DeltaTime
        {
            get { return m_DeltaTime; }
        }
    }
}
