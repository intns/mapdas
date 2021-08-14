namespace System.IO
{
	public class BigEndianWriter : BinaryWriter //By Wexos
	{
		public BigEndianWriter(Stream stream) : base(stream) { }
		public void WriteBoolean(bool Value)
		{
			base.Write(Value);
		}
		public void WriteByte(byte Value)
		{
			base.Write(Value);
		}
		public void WriteBytes(byte[] Value)
		{
			foreach (byte by in Value)
			{
				base.Write(by);
			}
		}
		public void WriteSByte(sbyte Value)
		{
			base.Write(Value);
		}
		public void WriteSBytes(sbyte[] Value)
		{
			foreach (sbyte by in Value)
			{
				base.Write(by);
			}
		}
		public void WriteUInt16(ushort Value)
		{
			byte[] Value2 = BitConverter.GetBytes(Value);
			Array.Reverse(Value2);
			base.Write(Value2);
		}
		public void WriteUInt16s(ushort[] Value)
		{
			foreach (ushort UInt in Value)
			{
				byte[] Value2 = BitConverter.GetBytes(UInt);
				Array.Reverse(Value2);
				base.Write(Value2);
			}
		}
		public void WriteInt16(short Value)
		{
			byte[] Value2 = BitConverter.GetBytes(Value);
			Array.Reverse(Value2);
			base.Write(Value2);
		}
		public void WriteInt16s(short[] Value)
		{
			foreach (short Int in Value)
			{
				byte[] Value2 = BitConverter.GetBytes(Int);
				Array.Reverse(Value2);
				base.Write(Value2);
			}
		}
		public void WriteUInt32(uint Value)
		{
			byte[] Value2 = BitConverter.GetBytes(Value);
			Array.Reverse(Value2);
			base.Write(Value2);
		}
		public void WriteUInt32s(uint[] Value)
		{
			foreach (uint UInt in Value)
			{
				byte[] Value2 = BitConverter.GetBytes(UInt);
				Array.Reverse(Value2);
				base.Write(Value2);
			}
		}
		public void WriteInt32(int Value)
		{
			byte[] Value2 = BitConverter.GetBytes(Value);
			Array.Reverse(Value2);
			base.Write(Value2);
		}
		public void WriteInt32s(int[] Value)
		{
			foreach (int Int in Value)
			{
				byte[] Value2 = BitConverter.GetBytes(Int);
				Array.Reverse(Value2);
				base.Write(Value2);
			}
		}
		public void WriteUInt64(ulong Value)
		{
			byte[] Value2 = BitConverter.GetBytes(Value);
			Array.Reverse(Value2);
			base.Write(Value2);
		}
		public void WriteUInt64s(ulong[] Value)
		{
			foreach (ulong UInt in Value)
			{
				byte[] Value2 = BitConverter.GetBytes(UInt);
				Array.Reverse(Value2);
				base.Write(Value2);
			}
		}
		public void WriteInt64(long Value)
		{
			byte[] Value2 = BitConverter.GetBytes(Value);
			Array.Reverse(Value2);
			base.Write(Value2);
		}
		public void WriteUInt64s(long[] Value)
		{
			foreach (long Int in Value)
			{
				byte[] Value2 = BitConverter.GetBytes(Int);
				Array.Reverse(Value2);
				base.Write(Value2);
			}
		}
		public void WriteSingle(float Value)
		{
			byte[] Value2 = BitConverter.GetBytes(Value);
			Array.Reverse(Value2);
			base.Write(Value2);
		}
		public void WriteSingles(float[] Value)
		{
			foreach (float fl in Value)
			{
				byte[] Value2 = BitConverter.GetBytes(fl);
				Array.Reverse(Value2);
				base.Write(Value2);
			}
		}
		public void WriteDecimal(decimal Value)
		{
			byte[] Value2 = BitConverter.GetBytes(Convert.ToDouble(Value));
			Array.Reverse(Value2);
			base.Write(Value2);
		}
		public void WriteDecimals(decimal[] Value)
		{
			foreach (decimal de in Value)
			{
				byte[] Value2 = BitConverter.GetBytes(Convert.ToInt64(de));
				Array.Reverse(Value2);
				base.Write(Value2);
			}
		}
		public void WriteString(string Value)
		{
			base.Write(Value);
		}
		public void WriteChar(char Value)
		{
			byte[] Value2 = BitConverter.GetBytes(Value);
			Array.Reverse(Value2);
			base.Write(Value2);
		}
		public void WriteChars(char[] Value)
		{
			foreach (char ch in Value)
			{
				byte[] Value2 = BitConverter.GetBytes(ch);
				Array.Reverse(Value2);
				base.Write(Value2);
			}
		}
		public void WriteChars(char[] Value, int index, int count)
		{
			base.Write(Value, index, count);
		}
	}
}
