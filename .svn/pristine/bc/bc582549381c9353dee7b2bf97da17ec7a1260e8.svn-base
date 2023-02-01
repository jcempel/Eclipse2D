using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eclipse2D.Network.Packets
{
    /// <summary>
    /// Represents a packet reader that can read raw data.
    /// </summary>
    public class PacketReader
    {
        /// <summary>
        /// The internal byte buffer which is being written to/ read from.
        /// </summary>
        private Byte[] BaseBuffer;

        /// <summary>
        /// The current position of the read needle.
        /// </summary>
        private Int32 ReadPosition;

        /// <summary>
        /// Represents the type of encoding used to encode/ decode strings.
        /// </summary>
        private Encoding StringEncoding;

        /// <summary>
        /// Initiates a new PacketReader with the specified pre-existing byte buffer.
        /// </summary>
        /// <param name="Buffer">The pre-existing byte array to use as the internal byte buffer.</param>
        public PacketReader(Byte[] Buffer)
            : this(Buffer, Encoding.Unicode)
        {

        }

        /// <summary>
        /// Initiates a new PacketReader with the specified pre-existing byte buffer with the specific string encoding.
        /// </summary>
        /// <param name="Buffer">The pre-existing byte array to use as the internal byte buffer.</param>
        /// <param name="EncodeType">The type of encoding to use on strings being sent over the network.</param>
        public PacketReader(Byte[] Buffer, Encoding EncodeType)
        {
            // Set the internal byte buffer.
            BaseBuffer = Buffer;

            // Set the encode type we're using on strings.
            StringEncoding = EncodeType;

            // Set the initial read position to zero.
            ReadPosition = 0;
        }

        /// <summary>
        /// Gets the base buffer or specific index.
        /// </summary>
        public Byte[] Buffer
        {
            get { return BaseBuffer; }
        }

        /// <summary>
        /// Reads a Byte from the internal byte array.
        /// </summary>
        /// <returns></returns>
        public Byte ReadByte()
        {
            return BaseBuffer[ReadPosition++];
        }

        /// <summary>
        /// Reads an Int16 from the internal byte array.
        /// </summary>
        /// <returns></returns>
        public Int16 ReadInt16()
        {
            return (Int16)((BaseBuffer[ReadPosition++]) | (BaseBuffer[ReadPosition++] << 8));
        }

        /// <summary>
        /// Reads an Int32 from the internal byte array.
        /// </summary>
        /// <returns></returns>
        public Int32 ReadInt32()
        {
            return ((BaseBuffer[ReadPosition++]) | (BaseBuffer[ReadPosition++] << 8) | (BaseBuffer[ReadPosition++] << 16) | (BaseBuffer[ReadPosition++] << 24));
        }

        /// <summary>
        /// Reads an Int64 from the internal byte array.
        /// </summary>
        /// <returns></returns>
        public Int64 ReadInt64()
        {
            return ((BaseBuffer[ReadPosition++]) | (BaseBuffer[ReadPosition++] << 8) | (BaseBuffer[ReadPosition++] << 16) | (BaseBuffer[ReadPosition++] << 24) |
                    (BaseBuffer[ReadPosition++] << 32) | (BaseBuffer[ReadPosition++] << 40) | (BaseBuffer[ReadPosition++] << 48) | (BaseBuffer[ReadPosition++] << 56));
        }

        /// <summary>
        /// Reads a Boolean from the internal byte array.
        /// </summary>
        /// <returns></returns>
        public Boolean ReadBoolean()
        {
            return BaseBuffer[ReadPosition++] != 0;
        }

        /// <summary>
        /// Reads a Unicode String from the internal byte array.
        /// </summary>
        /// <returns></returns>
        public String ReadString()
        {
            // Get the length of the string. This uses an internal call and auto-increments the read position.
            Int32 Length = ReadInt32();

            // Sometimes the string is empty, but valid, so we return an empty string.
            if (Length == 0)
                return String.Empty;

            // Get the encoded unicode string from the byte array.
            String Message = StringEncoding.GetString(BaseBuffer, ReadPosition, Length);

            // Increment the read position.
            ReadPosition = ReadPosition + Length;

            return Message;
        }
    }
}
