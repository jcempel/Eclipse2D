using System;
using System.Windows.Forms;
using System.Collections.Generic;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.MediaFoundation;
using SharpDX.Multimedia;

namespace Eclipse2D.Audio
{
    /// <summary>
    /// Represents an audio device, which can load, manage, and unload audio content.
    /// </summary>
    public class AudioDevice : IDisposable
    {
        /// <summary>
        /// Represents the XAudio2 device.
        /// </summary>
        private XAudio2 m_AudioDevice;

        /// <summary>
        /// Represents the audio output device (speakers, headset, etc).
        /// </summary>
        private MasteringVoice m_MasteringVoice;

        /// <summary>
        /// An event that fires after the audio engine has been loaded.
        /// </summary>
        public event EventHandler OnLoad;

        /// <summary>
        /// An event that fires before the audio engine is disposed.
        /// </summary>
        public event EventHandler OnUnload;

        /// <summary>
        /// Initializes a new AudioDevice class.
        /// </summary>
        public AudioDevice()
        {
            InitializeAudio(ProcessorSpecifier.DefaultProcessor);
        }

        /// <summary>
        /// Initializes a new AudioDevice class with the specified processor.
        /// </summary>
        /// <param name="Processor">The processor for the audio device.</param>
        public AudioDevice(ProcessorSpecifier Processor)
        {
            InitializeAudio(Processor);
        }

        /// <summary>
        /// Initializes the audio device.
        /// </summary>
        /// <param name="Processor">The processor for the audio device.</param>
        private void InitializeAudio(ProcessorSpecifier Processor)
        {
            // Initializes the XAudio2 engine using the default processor and the latest version installed.
            InitializeDevice(XAudio2Flags.None, Processor);

            // Determines is audio is enabled based on the device count.
            //if (m_AudioDevice.DeviceCount == 0)
            //{
            //    throw new AudioException("The audio device couldn't find any available output devices.");
            //}

            // Initialize the audio output device.
            m_MasteringVoice = new MasteringVoice(m_AudioDevice);

            // Let the listeners know that the audio engine has been loaded.
            OnLoad?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Initializes the XAudio2 engine using the specified flags and processor.
        /// </summary>
        /// <param name="Flags">Any flags to use.</param>
        /// <param name="Processor">The processor to host the audio device.</param>
        /// <remarks>
        /// The version requirements are as follows:
        ///     XAudio2 2.7 requires Windows 7+.
        ///     XAudio2 2.8 requires Windows 8+.
        ///     XAudio2 2.9 requires Windows 10+.
        /// </remarks>
        private void InitializeDevice(XAudio2Flags Flags, ProcessorSpecifier Processor)
        {
            try
            {
                // Try initializing XAudio2 2.9 (Windows 10+).
                m_AudioDevice = new XAudio2(Flags, Processor, XAudio2Version.Version29);
            }
            catch (DllNotFoundException)
            {
                try
                {
                    // Try initializing XAudio2 2.8 (Windows 8+).
                    m_AudioDevice = new XAudio2(Flags, Processor, XAudio2Version.Version28);
                }
                catch (DllNotFoundException)
                {
                    try
                    {
                        // Try initializing XAudio2 2.7 (Windows 7+).
                        m_AudioDevice = new XAudio2(Flags, Processor, XAudio2Version.Version27);
                    }
                    catch (DllNotFoundException)
                    {
                        // Throw an exception because XAudio2 wasn't found.
                        throw new AudioException("Failed to load XAudio2. No version of XAudio could be found.");
                    }
                }
            }

            // Hook the CriticalError event so the device and any audio can be reloaded.
            m_AudioDevice.CriticalError += AudioDevice_CriticalError;
        }

        /// <summary>
        /// Event: Executes when a critical error occurs in the audio engine.
        /// </summary>
        /// <param name="Sender">The object that created the event.</param>
        /// <param name="Args">The arguments passed by this event.</param>
        private void AudioDevice_CriticalError(Object Sender, ErrorEventArgs Args)
        {
            // TODO: Implement the reloading of the audio engine.
        }

        /// <summary>
        /// Disposes the audio device.
        /// </summary>
        public void Dispose()
        {
            // Let the listeners know that the audio device is closing.
            OnUnload?.Invoke(this, EventArgs.Empty);

            // Dispose objects.
            m_MasteringVoice.Dispose();
            m_AudioDevice.Dispose();
        }

        /// <summary>
        /// Gets the audio device.
        /// </summary>
        public XAudio2 Device
        {
            get
            {
                return m_AudioDevice;
            }
        }

        /// <summary>
        /// Gets the version of the audio device.
        /// </summary>
        public XAudio2Version DeviceVersion
        {
            get
            {
                return m_AudioDevice.Version;
            }
        }

        /// <summary>
        /// Gets the device the audio manager is using for output.
        /// </summary>
        public MasteringVoice OutputDevice
        {
            get
            {
                return m_MasteringVoice;
            }
        }

        /// <summary>
        /// Sets/Gets the master volume.
        /// </summary>
        public Single MasterVolume
        {
            set
            {
                m_MasteringVoice.SetVolume(value);
            }

            get
            {
                return m_MasteringVoice.Volume;
            }
        }

        /// <summary>
        /// Gets the count of available devices.
        /// </summary>
        public Int32 DeviceCount
        {
            get
            {
                return m_AudioDevice.DeviceCount;
            }
        }
    }
}
