using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SharpDX.XAudio2;
using SharpDX.Multimedia;

namespace Eclipse2D.Audio
{
    public class Sound
    {
        /// <summary>
        /// Represents the audio manager, which manages all sounds.
        /// </summary>
        private AudioDevice m_AudioManager;

        /// <summary>
        /// Represents the source voice, which submits audio data to the mastering voice (output device).
        /// </summary>
        private SourceVoice m_SourceVoice;

        /// <summary>
        /// Represents the buffer that contains the audio file loaded in memory.
        /// </summary>
        private AudioBuffer m_AudioBuffer;

        /// <summary>
        /// Represents the wave file format.
        /// </summary>
        private WaveFormat m_WaveFormat;

        /// <summary>
        /// Represents the decoded packets (used with XWMA file format).
        /// </summary>
        private UInt32[] m_DecodedPackets;

        /// <summary>
        /// Represents the current state of the sound.
        /// </summary>
        private AudioState m_AudioState;

        /// <summary>
        /// Represents the file name associated with this audio.
        /// </summary>
        private String m_FileName;

        /// <summary>
        /// Represents if the sound is repeating.
        /// </summary>
        private Boolean m_IsRepeating;

        /// <summary>
        /// Represents if this audio has been disposed.
        /// </summary>
        private Boolean m_IsDisposed;

        /// <summary>
        /// Initializes a new Sound class, which plays sound files.
        /// </summary>
        /// <param name="AudioMan">The audio manager associated with this sound.</param>
        public Sound(AudioDevice AudioMan)
        {
            // Set the AudioManager object.
            m_AudioManager = AudioMan;

            // Set the initial audio state.
            m_AudioState = AudioState.Stopped;

            // Executes when the audio manager is loaded.
            m_AudioManager.OnLoad += AudioManager_OnLoad;

            // Executes when the audio manager is unloaded.
            m_AudioManager.OnUnload += AudioManager_OnUnload;
        }

        /// <summary>
        /// Plays the sound.
        /// </summary>
        /// <param name="FileName">The specified sound file to play.</param>
        public void Play(String FileName)
        {
            // Checks if the sound is currently playing.
            if (m_AudioState == AudioState.Playing)
            {
                // Unload the sound, so it can be reloaded for another file.
                UnloadSound();
            }

            // Load the sound into memory.
            LoadSound(FileName);
        }

        /// <summary>
        /// Stops the sound.
        /// </summary>
        public void Stop()
        {
            // Checks if the audio is currently playing.
            if (m_AudioState == AudioState.Playing)
            {
                // Unload the sound.
                UnloadSound();
            }
        }

        /// <summary>
        /// Initializes the source voice and loads the audio into memory.
        /// </summary>
        /// <param name="FileName">The file name to load into memory.</param>
        private void LoadSound(String FileName)
        {
            // Sets the file name currently associated with this sound.
            m_FileName = FileName;

            // Open the audio file and read it into the sound stream.
            using (SoundStream Stream = new SoundStream(File.OpenRead(FileName)))
            {
                // Set the wave format.
                m_WaveFormat = Stream.Format;

                // Initialize the audio buffer using the open sound stream.
                m_AudioBuffer = new AudioBuffer
                {
                    // Convert the SoundStream to a DataStream.
                    Stream = Stream.ToDataStream(),

                    // The length of the stream.
                    AudioBytes = (Int32)Stream.Length,
                    
                    // Flag that no more data will be added after this load.
                    Flags = BufferFlags.EndOfStream
                };

                // Get the decoded packets. This is used only with the XWMA file format.
                m_DecodedPackets = Stream.DecodedPacketsInfo;
            }

            // Initialize the source voice.
            m_SourceVoice = new SourceVoice(m_AudioManager.Device, m_WaveFormat);

            // Set the audio state.
            m_AudioState = AudioState.Playing;

            // Submit the audio buffer to the source voice.
            m_SourceVoice.SubmitSourceBuffer(m_AudioBuffer, m_DecodedPackets);

            // Hook the BufferEnd event. The BufferEnd event is guaranteed to be called after the last
            // byte is consumed by the source voice, and before the next audio buffer is played. This allows
            // us to skip using a ring buffer and skip using an infinite loop to render audio.
            m_SourceVoice.BufferEnd += Sound_BufferEnd;

            // Starts the source voice.
            m_SourceVoice.Start();
        }

        /// <summary>
        /// Event: Executes when a buffer has ended (finished playing).
        /// </summary>
        /// <param name="Context"></param>
        private void Sound_BufferEnd(IntPtr Context)
        {
            // Checks if the sound is repeating.
            if (m_IsRepeating)
            {
                // Submit the audio buffer to the source voice.
                m_SourceVoice.SubmitSourceBuffer(m_AudioBuffer, m_DecodedPackets);
            }
            else
            {
                // Stop the source voice.
                m_SourceVoice.Stop();

                // Set the audio state.
                m_AudioState = AudioState.Stopped;
            }

        }

        /// <summary>
        /// Unloads the source voice and audio buffer.
        /// </summary>
        private void UnloadSound()
        {
            // Set the audio state.
            m_AudioState = AudioState.Stopped;

            // Stops the source voice.
            m_SourceVoice.Stop();

            // Flushes any existing buffers from the source voice.
            m_SourceVoice.FlushSourceBuffers();

            // Destroys the source voice.
            m_SourceVoice.DestroyVoice();

            // Dispose the audio stream.
            m_AudioBuffer.Stream.Dispose();

            // Dipose the source voice.
            m_SourceVoice.Dispose();
        }

        /// <summary>
        /// Event: Executes when the audio manager has been loaded.
        /// </summary>
        /// <param name="Sender">The object executing the event (AudioManager).</param>
        /// <param name="Args">The arguments passed by this event.</param>
        private void AudioManager_OnLoad(Object Sender, EventArgs Args)
        {
            // TODO: Support reloading of audio when the audio manager needs to be restarted.
        }

        /// <summary>
        /// Event: Executes when the audio manager is being unloaded.
        /// </summary>
        /// <param name="Sender">The object executing the event (AudioManager).</param>
        /// <param name="Args">The arguments passed by this event.</param>
        private void AudioManager_OnUnload(Object Sender, EventArgs Args)
        {
            // Checks if the audio has already been disposed.
            if (m_IsDisposed)
            {
                return;
            }

            // Unload the audio.
            UnloadSound();
        }


        /// <summary>
        /// Disposes the sound.
        /// </summary>
        public void Dispose()
        {
            // Disposes the audio.
            Dispose(true);

            // Inform the garbage collector that we don't need to finalize this object.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the sound.
        /// </summary>
        /// <param name="Disposing">Determines if the method was called programmatically (true) or by the garbage collector (false).</param>
        protected virtual void Dispose(Boolean Disposing)
        {
            // Checks if the sounds has already been disposed.
            if (!m_IsDisposed)
            {
                // Checks if the method was called programmatically (true) or by the garbage collector (false).
                if (Disposing)
                {
                    UnloadSound();
                }

                // Dispose of un-managed resources here.
            }

            m_IsDisposed = true;
        }

        /// <summary>
        /// Gets the current state for this sound.
        /// </summary>
        public AudioState AudioState
        {
            get
            {
                return m_AudioState;
            }
        }

        /// <summary>
        /// Sets/Gets whether or not this sound is repeating.
        /// </summary>
        public Boolean IsRepeating
        {
            set
            {
                m_IsRepeating = value;
            }

            get
            {
                return m_IsRepeating;
            }
        }
    }
}
