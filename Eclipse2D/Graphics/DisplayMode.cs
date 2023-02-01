using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eclipse2D.Graphics
{
    /// <summary>
    /// Represents an individual display mode
    /// </summary>
    public class DisplayMode
    {
        /// <summary>
        /// The width of the display mode.
        /// </summary>
        private Int32 m_Width;

        /// <summary>
        /// The height of the display mode.
        /// </summary>
        private Int32 m_Height;

        /// <summary>
        /// The format of the display mode.
        /// </summary>
        private SharpDX.DXGI.Format m_Format;

        /// <summary>
        /// Initializes a new DisplayMode with the specified width, height, and format.
        /// </summary>
        /// <param name="Width">The width of the display mode.</param>
        /// <param name="Height">The height of the display mode.</param>
        /// <param name="Format">The format of the display mode.</param>
        public DisplayMode(Int32 Width, Int32 Height, SharpDX.DXGI.Format Format)
        {
            m_Format = Format;
            m_Width = Width;
            m_Height = Height;
        }

        /// <summary>
        /// Compares two display modes and checks if they're equal.
        /// </summary>
        public static Boolean operator ==(DisplayMode Left, DisplayMode Right)
        {
            // Check if both objects are the same object.
            if (ReferenceEquals(Left, Right))
            {
                return true;
            }

            // Check if either object is null.
            if (ReferenceEquals(Left, null) || ReferenceEquals(Right, null))
            {
                return false;
            }

            return (Left.Format == Right.Format) && (Left.Width == Right.Width) && (Left.Height == Right.Height);
        }

        /// <summary>
        /// Compares two display modes and checks if they aren't equal.
        /// </summary>
        public static Boolean operator !=(DisplayMode Left, DisplayMode Right)
        {
            return !(Left == Right);
        }

        public override Boolean Equals(Object Obj)
        {
            return (Obj is DisplayMode) && (this == (DisplayMode)Obj);
        }

        public override Int32 GetHashCode()
        {
            return (m_Width.GetHashCode() ^ m_Height.GetHashCode() ^ m_Format.GetHashCode());
        }

        public override String ToString()
        {
            return String.Format("Width: {0}, Height: {1}, Format: {2}", m_Width, m_Height, m_Format);
        }

        /// <summary>
        /// Gets the width of the display mode.
        /// </summary>
        public Int32 Width
        {
            get
            {
                return m_Width;
            }
        }

        /// <summary>
        /// Gets the height of the display mode.
        /// </summary>
        public Int32 Height
        {
            get
            {
                return m_Height;
            }
        }

        /// <summary>
        /// Gets the format of the display mode.
        /// </summary>
        public SharpDX.DXGI.Format Format
        {
            get
            {
                return m_Format;
            }
        }
    }
}
