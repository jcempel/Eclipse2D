using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eclipse2D.Audio
{
    /// <summary>
    /// Represents an audio exception.
    /// </summary>
    public class AudioException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the AudioException class with the specified message.
        /// </summary>
        /// <param name="Message">The message describing the audio exception.</param>
        public AudioException(String Message)
            : base(Message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the AudioException class with the specified message and inner-exception.
        /// </summary>
        /// <param name="Message">The message describing the audio exception.</param>
        /// <param name="InnerException">The exception that caused this audio exception.</param>
        public AudioException(String Message, Exception InnerException)
            : base(Message, InnerException)
        {

        }
    }
}
