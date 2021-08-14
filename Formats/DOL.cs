using System;
using System.Collections.Generic;
using System.IO;

namespace mapdas
{
	internal class DOL
	{
		public enum SectionType
		{
			Text = 0,
			Data
		}

		public class Section
		{
			public long _Offset = 0;
			public long _Address = 0;
			public long _Size = 0;

			public MemoryStream _Data = null;

			public SectionType _Type = SectionType.Text;
		}

		public List<Section> _TextSections = new List<Section>();
		public List<Section> _DataSections = new List<Section>();
		private const int _MaxTextSections = 7;
		private const int _MaxDataSections = 11;
		private const long _AddressInfoLoc = 0x48;
		private const long _SizeInfoLoc = 0x90;
		private readonly long _OffsetInfoLoc = 0;
		private long _CurrLogicAddr = 0;

		public DOL(BigEndianReader reader)
		{
			for (int i = 0; i < _MaxTextSections + _MaxDataSections; i++)
			{
				reader.BaseStream.Seek(_OffsetInfoLoc + (i << 2), SeekOrigin.Begin);
				uint offset = reader.ReadUInt32();

				reader.BaseStream.Seek(_AddressInfoLoc + (i << 2), SeekOrigin.Begin);
				uint address = reader.ReadUInt32();

				reader.BaseStream.Seek(_SizeInfoLoc + (i << 2), SeekOrigin.Begin);
				uint size = reader.ReadUInt32();

				if (offset < 0x100)
				{
					continue;
				}

				reader.BaseStream.Seek(offset, SeekOrigin.Begin);

				Section newSection = new Section()
				{
					_Offset = offset,
					_Address = address,
					_Size = size,
					_Data = new MemoryStream(reader.ReadBytes(size)),
				};

				if (i < _MaxTextSections)
				{
					newSection._Type = SectionType.Text;
					_TextSections.Add(newSection);
				}
				else
				{
					newSection._Type = SectionType.Data;
					_DataSections.Add(newSection);
				}
			}

			_CurrLogicAddr = GetFirstSection()._Address;
			Seek(_CurrLogicAddr, SeekOrigin.Begin);
			reader.BaseStream.Seek(0, SeekOrigin.Begin);
		}

		public Section ResolveAddress(long gcAddr)
		{
			foreach (Section section in _TextSections)
			{
				if (section._Address <= gcAddr && gcAddr < section._Address + section._Size)
				{
					return section;
				}
			}

			foreach (Section section in _DataSections)
			{
				if (section._Address <= gcAddr && gcAddr < section._Address + section._Size)
				{
					return section;
				}
			}

			throw new Exception($"Unmapped Address {gcAddr:X}");
		}

		public void Seek(long where, SeekOrigin origin)
		{
			if (origin == SeekOrigin.Begin)
			{
				Section resolvedAddr = ResolveAddress(where);
				resolvedAddr._Data.Seek(where - resolvedAddr._Address, SeekOrigin.Begin);
				_CurrLogicAddr = where;
			}
			else if (origin == SeekOrigin.Current)
			{
				Section resolvedAddr = ResolveAddress(_CurrLogicAddr);
				resolvedAddr._Data.Seek((where + _CurrLogicAddr) - resolvedAddr._Address, SeekOrigin.Begin);
				_CurrLogicAddr += where;
			}
		}

		public byte[] Read(long size)
		{
			Section addr = ResolveAddress(_CurrLogicAddr);
			if (_CurrLogicAddr + size > addr._Address + addr._Size)
			{
				throw new Exception("Read goes over current section");
			}

			_CurrLogicAddr += size;
			BinaryReader beReader = new BinaryReader(addr._Data);
			return beReader.ReadBytes((int)size);
		}

		public Section GetFirstSection()
		{
			long smallestOffset = 0xffffffff;
			int idx = 0;
			SectionType type = SectionType.Text;

			for (int i = 0; i < _TextSections.Count; i++)
			{
				if (_TextSections[i]._Offset < smallestOffset)
				{
					smallestOffset = _TextSections[i]._Offset;
					idx = i;
					type = SectionType.Text;
				}
			}
			for (int i = 0; i < _DataSections.Count; i++)
			{
				if (_DataSections[i]._Offset < smallestOffset)
				{
					smallestOffset = _DataSections[i]._Offset;
					idx = i;
					type = SectionType.Data;
				}
			}

			return type == SectionType.Text ? _TextSections[idx] : _DataSections[idx];
		}
	}
}
