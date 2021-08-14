using System.Text;

namespace System.IO
{
	public class BigEndianReader : BinaryReader //By Wexos
	{
		public BigEndianReader(Stream stream) : base(stream) { }

		public override byte ReadByte()
		{
			return base.ReadByte();
		}
		public override byte[] ReadBytes(int Count)
		{
			byte[] value = new byte[Count];
			for (int i = 0; i < Count; i++)
			{
				value[i] = base.ReadByte();
			}
			return value;
		}
		public byte[] ReadBytes(uint Count)
		{
			byte[] value = new byte[Count];
			for (int i = 0; i < Count; i++)
			{
				value[i] = base.ReadByte();
			}
			return value;
		}
		public override sbyte ReadSByte()
		{
			return base.ReadSByte();
		}
		public sbyte[] ReadSBytes(int Count)
		{
			sbyte[] value = new sbyte[Count];
			for (int i = 0; i < Count; i++)
			{
				value[i] = base.ReadSByte();
			}
			return value;
		}
		public sbyte[] ReadSBytes(uint Count)
		{
			sbyte[] value = new sbyte[Count];
			for (int i = 0; i < Count; i++)
			{
				value[i] = base.ReadSByte();
			}
			return value;
		}
		public override ushort ReadUInt16()
		{
			byte[] value = base.ReadBytes(0x02);
			Array.Reverse(value);
			return BitConverter.ToUInt16(value, 0);
		}
		public ushort[] ReadUInt16s(int Count)
		{
			ushort[] value = new ushort[Count];
			for (int i = 0; i < Count; i++)
			{
				byte[] value2 = base.ReadBytes(0x02);
				Array.Reverse(value2);
				value[i] = BitConverter.ToUInt16(value2, 0);
			}
			return value;
		}
		public ushort[] ReadUInt16s(uint Count)
		{
			ushort[] value = new ushort[Count];
			for (int i = 0; i < Count; i++)
			{
				byte[] value2 = base.ReadBytes(0x02);
				Array.Reverse(value2);
				value[i] = BitConverter.ToUInt16(value2, 0);
			}
			return value;
		}
		public override short ReadInt16()
		{
			byte[] value = base.ReadBytes(0x02);
			Array.Reverse(value);
			return BitConverter.ToInt16(value, 0);
		}
		public short[] ReadInt16s(int Count)
		{
			short[] value = new short[Count];
			for (int i = 0; i < Count; i++)
			{
				byte[] value2 = base.ReadBytes(0x02);
				Array.Reverse(value2);
				value[i] = BitConverter.ToInt16(value2, 0);
			}
			return value;
		}
		public short[] ReadInt16s(uint Count)
		{
			short[] value = new short[Count];
			for (int i = 0; i < Count; i++)
			{
				byte[] value2 = base.ReadBytes(0x02);
				Array.Reverse(value2);
				value[i] = BitConverter.ToInt16(value2, 0);
			}
			return value;
		}
		public override uint ReadUInt32()
		{
			byte[] value = base.ReadBytes(0x04);
			Array.Reverse(value);
			return BitConverter.ToUInt32(value, 0);
		}
		public uint[] ReadUInt32s(int Count)
		{
			uint[] value = new uint[Count];
			for (int i = 0; i < Count; i++)
			{
				byte[] value2 = base.ReadBytes(0x04);
				Array.Reverse(value2);
				value[i] = BitConverter.ToUInt32(value2, 0);
			}
			return value;
		}
		public uint[] ReadUInt32s(uint Count)
		{
			uint[] value = new uint[Count];
			for (int i = 0; i < Count; i++)
			{
				byte[] value2 = base.ReadBytes(0x04);
				Array.Reverse(value2);
				value[i] = BitConverter.ToUInt32(value2, 0);
			}
			return value;
		}
		public override int ReadInt32()
		{
			byte[] value = base.ReadBytes(0x04);
			Array.Reverse(value);
			return BitConverter.ToInt32(value, 0);
		}
		public int[] ReadInt32s(int Count)
		{
			int[] value = new int[Count];
			for (int i = 0; i < Count; i++)
			{
				byte[] value2 = base.ReadBytes(0x04);
				Array.Reverse(value2);
				value[i] = BitConverter.ToInt32(value2, 0);
			}
			return value;
		}
		public int[] ReadInt32s(uint Count)
		{
			int[] value = new int[Count];
			for (int i = 0; i < Count; i++)
			{
				byte[] value2 = base.ReadBytes(0x04);
				Array.Reverse(value2);
				value[i] = BitConverter.ToInt32(value2, 0);
			}
			return value;
		}
		public override ulong ReadUInt64()
		{
			byte[] value = base.ReadBytes(0x08);
			Array.Reverse(value);
			return BitConverter.ToUInt64(value, 0);
		}
		public ulong[] ReadUInt64s(int Count)
		{
			ulong[] value = new ulong[Count];
			for (int i = 0; i < Count; i++)
			{
				byte[] value2 = base.ReadBytes(0x08);
				Array.Reverse(value2);
				value[i] = BitConverter.ToUInt64(value2, 0);
			}
			return value;
		}
		public ulong[] ReadUInt64s(uint Count)
		{
			ulong[] value = new ulong[Count];
			for (int i = 0; i < Count; i++)
			{
				byte[] value2 = base.ReadBytes(0x08);
				Array.Reverse(value2);
				value[i] = BitConverter.ToUInt64(value2, 0);
			}
			return value;
		}
		public override long ReadInt64()
		{
			byte[] value = base.ReadBytes(0x08);
			Array.Reverse(value);
			return BitConverter.ToInt64(value, 0);
		}
		public long[] ReadInt64s(int Count)
		{
			long[] value = new long[Count];
			for (int i = 0; i < Count; i++)
			{
				byte[] value2 = base.ReadBytes(0x08);
				Array.Reverse(value2);
				value[i] = BitConverter.ToInt64(value2, 0);
			}
			return value;
		}
		public long[] ReadInt64s(uint Count)
		{
			long[] value = new long[Count];
			for (int i = 0; i < Count; i++)
			{
				byte[] value2 = base.ReadBytes(0x08);
				Array.Reverse(value2);
				value[i] = BitConverter.ToInt64(value2, 0);
			}
			return value;
		}
		public override float ReadSingle()
		{
			byte[] value = base.ReadBytes(0x04);
			Array.Reverse(value);
			return BitConverter.ToSingle(value, 0);
		}
		public float[] ReadSingles(int Count)
		{
			float[] value = new float[Count];
			for (int i = 0; i < Count; i++)
			{
				byte[] value2 = base.ReadBytes(0x04);
				Array.Reverse(value2);
				value[i] = BitConverter.ToSingle(value2, 0);
			}
			return value;
		}
		public float[] ReadSingles(uint Count)
		{
			float[] value = new float[Count];
			for (int i = 0; i < Count; i++)
			{
				byte[] value2 = base.ReadBytes(0x04);
				Array.Reverse(value2);
				value[i] = BitConverter.ToSingle(value2, 0);
			}
			return value;
		}
		public override string ReadString()
		{
			return base.ReadString();
		}
		public string ReadASCII(int count)
		{
			return Encoding.ASCII.GetString(base.ReadBytes(count));
		}
		public string ReadASCII(uint count)
		{
			return Encoding.ASCII.GetString(base.ReadBytes(Convert.ToInt32(count)));
		}
		public string[] ReadStrings(int Count)
		{
			string[] value = new string[Count];
			for (int i = 0; i < Count; i++)
			{
				value[i] = base.ReadString();
			}
			return value;
		}
	}
}
