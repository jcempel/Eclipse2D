﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.MediaFoundation;
using SharpDX.IO;

namespace Eclipse2D.Audio
{
    /// <summary>
    /// Represents an audio file.
    /// </summary>
    public class Music : IDisposable
    {
        /// <summary>
        /// Represents the audio device, which manages all audio samples.
        /// </summary>
        private AudioDevice m_AudioDevice;

        /// <summary>
        /// Represents the source voice, which submits audio data to the mastering voice (output device).
        /// </summary>
        private SourceVoice m_SourceVoice;

        /// <summary>
        /// Represents the audio decoder, which provides decoding for popular audio files.
        /// </summary>
        private AudioDecoder m_AudioDecoder;

        /// <summary>
        /// Provides a native file stream to the audio file.
        /// </summary>
        private NativeFileStream m_FileStream;

        /// <summary>
        /// Represents an iterator, which iterates over our audio samples.
        /// </summary>
        private IEnumerator<DataPointer> m_AudioData;

        /// <summary>
        /// Represents the current state of the audio.
        /// </summary>
        private AudioState m_AudioState;

        /// <summary>
        /// Represents the file name of the audio in memory.
        /// </summary>
        private String m_FileName;

        /// <summary>
        /// Represents if the audio is repeating.
        /// </summary>
        private Boolean m_IsRepeating;

        /// <summary>
        /// Represents if this audio has been disposed.
        /// </summary>
        private Boolean m_IsDisposed;

        /// <summary>
        /// Initializes a new Music class, which plays audio files.
        /// </summary>
        /// <param name="AudioMan">The audio device associated with this audio file.</param>
        public Music(AudioDevice AudioDevice)
        {
            // Set the device object.
            m_AudioDevice = AudioDevice;

            // Set the initial audio state.
            m_AudioState = AudioState.Stopped;

            // Executes when the audio device is loaded.
            m_AudioDevice.OnLoad += AudioManager_OnLoad;

            // Executes when the audio device is unloaded.
            m_AudioDevice.OnUnload += AudioManager_OnUnload;
        }

        /// <summary>
        /// Plays the audio.
        /// </summary>
        /// <param name="FileName">The specified audio file to play.</param>
        public void Play(String FileName)
        {
            // Checks if the audio is currently playing.
            if (m_AudioState == AudioState.Playing)
            {
                // Unload the audio, so it can be reloaded for another file.
                UnloadAudio();
            }

            // Load the audio into memory.
            LoadAudio(FileName);
        }

        /// <summary>
        /// Stops the audio.
        /// </summary>
        public void Stop()
        {
            // Checks if the audio is currently playing.
            if (m_AudioState == AudioState.Playing)
            {
                // Unload the audio.
                UnloadAudio();
            }
        }

        /// <summary>
        /// Initializes the source voice, loads the audio into memory, and decodes the audio samples.
        /// </summary>
        /// <param name="FileName">The file name to load into memory.</param>
        private void LoadAudio(String FileName)
        {
            // Set the file name currently associated with this audio.
            m_FileName = FileName;

            // Loads the file into memory.
            m_FileStream = new NativeFileStream(FileName, NativeFileMode.Open, NativeFileAccess.Read);

            // Decodes the audio file and provides us with audio samples.
            m_AudioDecoder = new AudioDecoder(m_FileStream);

            // Initialize the source voice using the PCM wave format provided by the audio decoder.
            m_SourceVoice = new SourceVoice(m_AudioDevice.Device, m_AudioDecoder.WaveFormat);

            // Get the enumerator so we can start iterating the samples provided by the audio decoder.
            m_AudioData = m_AudioDecoder.GetSamples().GetEnumerator();

            // Set the audio state.
            m_AudioState = AudioState.Playing;

            // Begin iterating over the samples.
            if (m_AudioData.MoveNext())
            {
                // Submit the first sample to the source voice.
                m_SourceVoice.SubmitSourceBuffer(new AudioBuffer(m_AudioData.Current), null);
            }

            // Hook the BufferEnd event. The BufferEnd event is guaranteed to be called after the last
            // byte is consumed by the source voice, and before the next audio buffer is played. This allows
            // us to skip using a ring buffer and skip using an infinite loop to render audio.
            m_SourceVoice.BufferEnd += Audio_BufferEnd;

            // Starts the source voice.
            m_SourceVoice.Start();
        }

        /// <summary>
        /// Unloads the source voice, decoder, and file stream.
        /// </summary>
        private void UnloadAudio()
        {
            // Set the audio state.
            m_AudioState = AudioState.Stopped;

            // Stops the source voice.
            m_SourceVoice.Stop();

            // Flushes any existing buffers from the source voice.
            m_SourceVoice.FlushSourceBuffers();

            // Destroys the source voice.
            m_SourceVoice.DestroyVoice();

            // Set the iterator.
            m_AudioData = null;

            // Dispose objects.
            m_SourceVoice.Dispose();
            m_AudioDecoder.Dispose();
            m_FileStream.Dispose();
        }

        /// <summary>
        /// Event: Executes when a buffer has ended (finished playing).
        /// </summary>
        /// <param name="Context"></param>
        private void Audio_BufferEnd(IntPtr Context)
        {
            // Begin iterating over the samples.
            if (m_AudioData.MoveNext())
            {
                // Submit the next sample to the source voice.
                m_SourceVoice.SubmitSourceBuffer(new AudioBuffer(m_AudioData.Current), null);
            }
            else
            {
                // Check if the audio is repeating.
                if (m_IsRepeating)
                {
                    // Get the enumerator so we can start iterating the samples provided by the audio decoder.
                    m_AudioData = m_AudioDecoder.GetSamples().GetEnumerator();

                    // Begin iterating over the samples.
                    if (m_AudioData.MoveNext())
                    {
                        // Submit the first sample to the source voice.
                        m_SourceVoice.SubmitSourceBuffer(new AudioBuffer(m_AudioData.Current), null);
                    }
                }
                else
                {
                    // Set the audio state.
                    m_AudioState = AudioState.Stopped;
                }
            }
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
            UnloadAudio();
        }

        /// <summary>
        /// Disposes the audio.
        /// </summary>
        public void Dispose()
        {
            // Disposes the audio.
            Dispose(true);

            // Inform the garbage collector that we don't need to finalize this object.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the audio.
        /// </summary>
        /// <param name="Disposing">Determines if the method was called programmatically (true) or by the garbage collector (false).</param>
        protected virtual void Dispose(Boolean Disposing)
        {
            // Checks if the audio has already been disposed.
            if (!m_IsDisposed)
            {
                // Checks if the method was called programmatically (true) or by the garbage collector (false).
                if (Disposing)
                {
                    UnloadAudio();
                }

                // Dispose of un-managed resources here.
            }

            m_IsDisposed = true;
        }

        /// <summary>
        /// Gets the current state for this audio.
        /// </summary>
        public AudioState AudioState
        {
            get
            {
                return m_AudioState;
            }
        }

        /// <summary>
        /// Sets/Gets the current volume.
        /// </summary>
        public Single Volume
        {
            set
            {
                m_SourceVoice.SetVolume(value);
            }

            get
            {
                return m_SourceVoice.Volume;
            }
        }

        /// <summary>
        /// Gets the file name associated with this audio.
        /// </summary>
        public String FileName
        {
            get
            {
                return m_FileName;
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
