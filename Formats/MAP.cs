using System.Collections.Generic;
using System.Linq;

namespace mapdas
{
	internal class MAP
	{
		public enum SectionType
		{
			Init = 0,
			Extab,
			ExtabIndex,
			Text,
			Ctors,
			Dtors,
			Rodata,
			Data,
			Bss,
			Sdata,
			Sbss,
			Sdata2,
			Sbss2,
			MemoryMap,
			LinkerSymbols,
			LinkerMap,
		}

		private readonly Dictionary<SectionType, string> _SectionConverter = new Dictionary<SectionType, string>(16) { { SectionType.Init, ".init section layout" }, { SectionType.Extab, "extab section layout" }, { SectionType.ExtabIndex, "extabindex section layout" }, { SectionType.Text, ".text section layout" }, { SectionType.Ctors, ".ctors section layout" }, { SectionType.Dtors, ".dtors section layout" }, { SectionType.Rodata, ".rodata section layout" }, { SectionType.Data, ".data section layout" }, { SectionType.Bss, ".bss section layout" }, { SectionType.Sbss, ".sbss section layout" }, { SectionType.Sbss2, ".sbss2 section layout" }, { SectionType.Sdata, ".sdata section layout" }, { SectionType.Sdata2, ".sdata2 section layout" }, { SectionType.MemoryMap, "Memory map:" }, { SectionType.LinkerMap, "Link map of __start" }, { SectionType.LinkerSymbols, "Linker generated symbols:" },
		};
		private readonly Dictionary<string, string[]> _Sections = new Dictionary<string, string[]>();

		public List<MAPParsing.TextEntry> _TextSymbols = new List<MAPParsing.TextEntry>();
		public DOL _AssociatedDol = null;

		public MAP(string[] lines)
		{
			List<string> currentSegmentEntries = new List<string>();
			string currentSegment = "";

			foreach (string line in lines)
			{
				if (string.IsNullOrEmpty(line))
				{
					if (currentSegmentEntries.Any())
					{
						_Sections[currentSegment] = currentSegmentEntries.ToArray();
						currentSegmentEntries.Clear();

						continue;
					}

					continue;
				}

				if (line[0] != ' ')
				{
					currentSegment = line.TrimStart();
					continue;
				}

				currentSegmentEntries.Add(line);
			}
			if (currentSegmentEntries.Any())
			{
				_Sections[currentSegment] = currentSegmentEntries.ToArray();
			}

			_TextSymbols = MAPParsing.ParseTextSection(this);
		}

		public string[] TryGetSection(SectionType type)
		{
			return _Sections.TryGetValue(_SectionConverter[type], out string[] result) ? result : null;
		}

		public string[] TryGetSection(string type)
		{
			return _Sections.TryGetValue(type, out string[] result) ? result : null;
		}
	}
}
