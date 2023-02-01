using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eclipse2D.Network.Packets
{
    /// <summary>
    /// Represents a packet writer that can write raw data.
    /// </summary>
    public class PacketWriter
    {
        /// <summary>
        /// The internal byte buffer which is being written to/ read from.
        /// </summary>
        private Byte[] BaseBuffer;

        /// <summary>
        /// The current position of the write needle.
        /// </summary>
        private Int32 WritePosition;

        /// <summary>
        /// Represents the type of encoding used to encode/ decode strings.
        /// </summary>
        private Encoding StringEncoding;

        /// <summary>
        /// Initiates a new PacketWriter with an initial buffer size.
        /// </summary>
        public PacketWriter()
            : this(new Byte[0])
        {

        }

        /// <summary>
        /// Initiates a new PacketWriter with the specified pre-existing byte buffer.
        /// </summary>
        /// <param name="Buffer">The pre-existing byte array to use as the internal byte buffer.</param>
        public PacketWriter(Byte[] Buffer)
            : this(Buffer, Encoding.Unicode)
        {

        }

        /// <summary>
        /// Initiates a new PacketWriter with the specified pre-existing byte buffer with the specific string encoding.
        /// </summary>
        /// <param name="Buffer">The pre-existing byte array to use as the internal byte buffer.</param>
        /// <param name="EncodeType">The type of encoding to use on strings being sent over the network.</param>
        public PacketWriter(Byte[] Buffer, Encoding EncodeType)
        {
            // Set the internal byte buffer.
            BaseBuffer = Buffer;

            // Set the encode type we're using on strings.
            StringEncoding = EncodeType;

            // Set the initial write position to zero.
            WritePosition = 0;
        }

        /// <summary>
        /// Converts the internal buffer into a packet buffer. This method is used for
        /// future changes in the packet buffer class if the network engine changes.
        /// </summary>
        /// <returns></returns>
        public Byte[] ToNetworkBuffer()
        {
            return BaseBuffer;
        }

        /// <summary>
        /// Writes a Byte to the internal byte buffer.
        /// </summary>
        /// <param name="Value"></param>
        public void WriteByte(Byte Value)
        {
            Buffer_IncreaseSize(1);
            BaseBuffer[WritePosition++] = Value;
        }

        /// <summary>
        /// Write an Int16 to the internal byte buffer.
        /// </summary>
        /// <param name="Value"></param>
        public void WriteInt16(Int16 Value)
        {
            Buffer_IncreaseSize(2);
            BaseBuffer[WritePosition++] = (Byte)(Value);
            BaseBuffer[WritePosition++] = (Byte)(Value >> 8);
        }

        /// <summary>
        /// Write an Int32 to the internal byte buffer.
        /// </summary>
        /// <param name="Value"></param>
        public void WriteInt32(Int32 Value)
        {
            Buffer_IncreaseSize(4);
            BaseBuffer[WritePosition++] = (Byte)(Value);
            BaseBuffer[WritePosition++] = (Byte)(Value >> 8);
            BaseBuffer[WritePosition++] = (Byte)(Value >> 16);
            BaseBuffer[WritePosition++] = (Byte)(Value >> 24);
        }

        /// <summary>
        /// Write an Int64 to the internal byte buffer.
        /// </summary>
        /// <param name="Value"></param>
        public void WriteInt64(Int64 Value)
        {
            Buffer_IncreaseSize(8);
            BaseBuffer[WritePosition++] = (Byte)(Value);
            BaseBuffer[WritePosition++] = (Byte)(Value >> 8);
            BaseBuffer[WritePosition++] = (Byte)(Value >> 16);
            BaseBuffer[WritePosition++] = (Byte)(Value >> 24);
            BaseBuffer[WritePosition++] = (Byte)(Value >> 32);
            BaseBuffer[WritePosition++] = (Byte)(Value >> 40);
            BaseBuffer[WritePosition++] = (Byte)(Value >> 48);
            BaseBuffer[WritePosition++] = (Byte)(Value >> 56);
        }

        /// <summary>
        /// Write a Boolean to the internal byte array.
        /// </summary>
        /// <param name="Value"></param>
        public void WriteBoolean(Boolean Value)
        {
            Buffer_IncreaseSize(1);
            BaseBuffer[WritePosition++] = (Byte)(Value ? 1 : 0);
        }

        /// <summary>
        /// Writes a Unicode String to the internal byte array.
        /// </summary>
        /// <param name="Value"></param>
        public void WriteString(String Value)
        {
            Int32 Length = StringEncoding.GetByteCount(Value);

            // Write the string length before the string data, so we can decode it on the other end.
            WriteInt32(Length);

            // Sometimes the string is empty, but valid, so all we need is the string length.
            if (Length > 0)
            {
                Buffer_IncreaseSize(Length);
                Buffer.BlockCopy(StringEncoding.GetBytes(Value), 0, BaseBuffer, WritePosition, Length);
                WritePosition = WritePosition + Length;
            }
        }

        /// <summary>
        /// Increases the size of the internal byte buffer.
        /// </summary>
        /// <param name="Bytes">The amount of bytes to increase in size by.</param>
        private void Buffer_IncreaseSize(Int32 Bytes)
        {
            // Create the new byte array with the new size.
            Byte[] NewBuffer = new Byte[BaseBuffer.Length + Bytes];

            // Copy the data from the old byte array to the new one.
            Buffer.BlockCopy(BaseBuffer, 0, NewBuffer, 0, BaseBuffer.Length);

            // Switch the buffers.
            BaseBuffer = NewBuffer;
        }

        /// <summary>
        /// Gets the length, in bytes, of the packet.
        /// </summary>
        public Int32 Length
        {
            get
            {
                return BaseBuffer.Length;
            }
        }
    }
}
