using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DXGI;
using SharpDX.IO;
using SharpDX.WIC;
using SharpDX.Windows;

namespace Eclipse2D.Graphics
{
    /// <summary>
    /// Represents a texture that can be drawn to the game screen.
    /// </summary>
    public class Texture2D : IDisposable
    {
        /// <summary>
        /// Represents the texture in memory.
        /// </summary>
        private Bitmap1 m_BitmapTexture;

        /// <summary>
        /// Represents the device associated with this texture.
        /// </summary>
        private GraphicsDevice m_GraphicsDevice;

        /// <summary>
        /// Represents the filename associated with this texture.
        /// </summary>
        private String m_FileName;

        /// <summary>
        /// Represents if the texture has been disposed.
        /// </summary>
        private Boolean m_IsDisposed;

        /// <summary>
        /// Initializes a new Texture2D with the specified device context and filename.
        /// </summary>
        /// <param name="D2D1DeviceContext">The graphics device loading the texture.</param>
        /// <param name="FileName">The filename to load into memory.</param>
        public Texture2D(GraphicsDevice Device, String FileName)
        {
            // Set the device context reference.
            m_GraphicsDevice = Device;

            // Set the texture bitmap filename.
            m_FileName = FileName;

            // Load the bitmap texture into memory.
            m_BitmapTexture = LoadFromFile(FileName);
        }

        /// <summary>
        /// Loads a bitmap texture into memory.
        /// </summary>
        /// <param name="FileName">The filename the load into memory.</param>
        /// <returns></returns>
        private Bitmap1 LoadFromFile(String FileName)
        {
            Bitmap1 LoadedBitmap = null;

            // Initializes the WIC imaging factory.
            using (ImagingFactory ImagingFactory = new ImagingFactory())
            {
                // Initializes the stream which will load the bitmap into memory.
                using (NativeFileStream BitmapFileStream = new NativeFileStream(Environment.CurrentDirectory + FileName, NativeFileMode.Open, NativeFileAccess.Read))
                {
                    // Initializes the decoder to decode the bitmap being loaded.
                    using (BitmapDecoder Decoder = new BitmapDecoder(ImagingFactory, BitmapFileStream, DecodeOptions.CacheOnDemand))
                    {
                        // Initializes the converter.
                        using (FormatConverter Converter = new FormatConverter(ImagingFactory))
                        {
                            // Initializes the converter with the bitmap source and format.
                            Converter.Initialize(Decoder.GetFrame(0), SharpDX.WIC.PixelFormat.Format32bppPRGBA, BitmapDitherType.None, null, 0.0D, BitmapPaletteType.Custom);

                            // Create the new bitmap from the converter.
                            LoadedBitmap = Bitmap1.FromWicBitmap(m_GraphicsDevice.D2DDeviceContext, Converter);
                        }
                    }
                }
            }

            return LoadedBitmap;
        }
        /// <summary>
        /// Gets the center position of the texture.
        /// </summary>
        public Vector2 Center
        {
            get
            {
                return new Vector2(m_BitmapTexture.Size.Width / 2.0F, m_BitmapTexture.Size.Height / 2.0F);
            }
        }

        /// <summary>
        /// Disposes the texture.
        /// </summary>
        public void Dispose()
        {
            // Disposes the texture.
            Dispose(true);

            // Inform the garbage collector that we don't need to finalize this object.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the texture.
        /// </summary>
        /// <param name="Disposing">Determines if the method was called programmatically (true) or by the garbage collector (false).</param>
        protected virtual void Dispose(Boolean Disposing)
        {
            // Checks if the texture has already been disposed.
            if (!m_IsDisposed)
            {
                // Checks if the method was called programmatically (true) or by the garbage collector (false).
                if (Disposing)
                {
                    m_BitmapTexture.Dispose();
                }

                // Dispose of un-managed resources here.
            }

            m_IsDisposed = true;
        }

        /// <summary>
        /// Gets the graphics device associated with the texture.
        /// </summary>
        public GraphicsDevice Device
        {
            get
            {
                return m_GraphicsDevice;
            }
        }

        /// <summary>
        /// Gets the underlying bitmap associated with the texture.
        /// </summary>
        public Bitmap1 Bitmap
        {
            get
            {
                return m_BitmapTexture;
            }
        }

        /// <summary>
        /// Gets the file name associated with the texture.
        /// </summary>
        public String FileName
        {
            get
            {
                return m_FileName;
            }
        }

        /// <summary>
        /// Gets the pixel format of the texture.
        /// </summary>
        public SharpDX.Direct2D1.PixelFormat Format
        {
            get
            {
                return m_BitmapTexture.PixelFormat;
            }
        }

        /// <summary>
        /// Gets the size, in device-independent pixels (DIPS), of the texture.
        /// </summary>
        public Size2F Size
        {
            get
            {
                return m_BitmapTexture.Size;
            }
        }

        /// <summary>
        /// Gets the size, in device-dependent pixels (physical pixels), of the texture.
        /// </summary>
        public Size2 PixelSize
        {
            get
            {
                return m_BitmapTexture.PixelSize;
            }
        }
    }
}
