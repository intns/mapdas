using arookas;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace mapdas
{
	internal static class MAPParsing
	{
		public class TextEntry
		{
			public string _Path = string.Empty;

			public string _Address = string.Empty;
			public string _Start = string.Empty;
			public string _Size = string.Empty;

			public string _Type = string.Empty;

			public string _Symbol = string.Empty;
			public string _SymbolDemangled = string.Empty;
		}

		public class MemoryMapEntry
		{
			public string _Name = string.Empty;
			public string _Start = string.Empty;
			public string _Size = string.Empty;
			public string _Offset = string.Empty;
		}

		public class LinkerSymbolEntry
		{
			public string _Name = string.Empty;
			public string _Start = string.Empty;
		}

		public static List<TextEntry> ParseTextSection(MAP file)
		{
			string[] segmentLines = file.TryGetSection(MAP.SectionType.Text);
			if (segmentLines == null)
			{
				return null;
			}

			bool updatedMap = segmentLines[0].Contains("File");

			List<TextEntry> symbols = new List<TextEntry>();
			foreach (string line in segmentLines)
			{
				// Skip ".text" symbols and empty lines
				if (line.Contains(".text") || string.IsNullOrWhiteSpace(line))
				{
					continue;
				}

				// st = start
				// si = size
				// addr = address
				// offs = offset
				// t = type
				// fun_n = function name
				// fol_n = folder name
				// fil_n = file name

				Match symbol = null;
				if (!updatedMap)
				{
					// folder & used
					Match fu_sym = Regex.Match(line, @"\s(?<st>[a-f0-9]+)\s+(?<si>[a-f0-9]+)\s+(?<addr>[a-f0-9]+)\s+(?<t>\d+)\s+(?<fun_n>[^ ]+)\s+(?<fol_n>[^ ]+)?\s(?<fil_n>[^ ]+)", RegexOptions.Compiled);
					// unused & folder
					Match uf_sym = Regex.Match(line, @"\s(?<st>[A-Z]+)\s+(?<si>[a-f0-9]+)\s+(?<addr>[.]+)\s+(?<fun_n>[^ ]+)\s+(?<fol_n>[^ ]+)?\s(?<fil_n>[^ ]+)", RegexOptions.Compiled);
					// unused & no folder
					Match unf_sym = Regex.Match(line, @"\s(?<st>[A-Z]+)\s+(?<si>[a-f0-9]+)\s+(?<addr>[.]+)\s+(?<fun_n>[^ ]+)?\s(?<fil_n>[^ ]+)", RegexOptions.Compiled);
					if (!fu_sym.Success && !uf_sym.Success && !unf_sym.Success)
					{
						continue;
					}

					if (fu_sym.Success)
					{
						symbol = fu_sym;
					}
					else if (uf_sym.Success)
					{
						symbol = uf_sym;
					}
					else if (unf_sym.Success)
					{
						symbol = unf_sym;
					}
				}
				else // Updated map, thanks wowjinxy for showing me these
				{
					// used & folder
					Match fu_sym = Regex.Match(line, @"\s(?<st>[a-f0-9]+)\s+(?<si>[a-f0-9]+)\s+(?<addr>[a-f0-9]+)\s+(?<offs>[a-f0-9]+)\s+(?<t>\d+)\s+(?<fun_n>[^ ]+)\s+(?<fol_n>[^ ]+)?\s(?<fil_n>[^ ]+)", RegexOptions.Compiled);
					// unused & folder
					Match uf_sym = Regex.Match(line, @"\s(?<st>[A-Z]+)\s+(?<si>[a-f0-9]+)\s+(?<addr>[.]+)\s+(?<offs>[a-f0-9]+)\s+(?<fun_n>[^ ]+)\s+(?<fol_n>[^ ]+)?\s(?<fil_n>[^ ]+)", RegexOptions.Compiled);
					// unused & no folder
					Match unf_sym = Regex.Match(line, @"\s(?<st>[A-Z]+)\s+(?<si>[a-f0-9]+)\s+(?<addr>[.]+)\s+(?<offs>[a-f0-9]+)\s+(?<fun_n>[^ ]+)?\s(?<fil_n>[^ ]+)", RegexOptions.Compiled);
					if (!fu_sym.Success && !uf_sym.Success && !unf_sym.Success)
					{
						continue;
					}

					if (fu_sym.Success)
					{
						symbol = fu_sym;
					}
					else if (uf_sym.Success)
					{
						symbol = uf_sym;
					}
					else if (unf_sym.Success)
					{
						symbol = unf_sym;
					}
				}

				TextEntry newTextSymbol = new TextEntry
				{
					_Address = symbol.Groups["addr"].Value,
					_Start = symbol.Groups["st"].Value,
					_Size = symbol.Groups["si"].Value,
					_Type = symbol.Groups["t"].Value,
					_Symbol = symbol.Groups["fun_n"].Value,
					_SymbolDemangled = Demangler.Demangle(symbol.Groups["fun_n"].Value),
				};

				// If the .o file uses an absolute path we just recreate the entire directory 
				// because fuck it.
				if (!symbol.Groups["fil_n"].Value.Contains(":\\"))
				{
					string folder = symbol.Groups["fol_n"].Value.Replace(".o", "").Replace(".a", "").Replace('.', '/');
					if (!folder.EndsWith("/"))
					{
						folder += "/";
					}
					newTextSymbol._Path = folder + symbol.Groups["fil_n"].Value.Replace(".o", ".cpp");
				}
				else
				{
					newTextSymbol._Path = symbol.Groups["fil_n"].Value.Replace(".o", ".cpp");
				}

				symbols.Add(newTextSymbol);
			}
			return symbols;
		}

		public static List<MemoryMapEntry> ParseMemoryMapSection(MAP file)
		{
			string[] segmentLines = file.TryGetSection(MAP.SectionType.MemoryMap);
			if (segmentLines == null)
			{
				return null;
			}

			List<MemoryMapEntry> symbols = new List<MemoryMapEntry>();
			foreach (string line in segmentLines)
			{
				Match match = Regex.Match(line, @"\s+(?<name>[^ ]+)\s+(?<start>[a-f0-9]+)\s+(?<size>[a-f0-9]+)\s+(?<offset>[a-f0-9]+)", RegexOptions.Compiled);
				if (match.Success)
				{
					symbols.Add(new MemoryMapEntry
					{
						_Name = match.Groups["name"].Value,
						_Start = match.Groups["start"].Value.Trim(),
						_Size = match.Groups["size"].Value,
						_Offset = match.Groups["offset"].Value,
					});
				}
			}

			return symbols;
		}

		public static List<LinkerSymbolEntry> ParseLinkerSymbolSection(MAP file)
		{
			string[] segmentLines = file.TryGetSection(MAP.SectionType.LinkerSymbols);
			if (segmentLines == null)
			{
				return null;
			}

			List<LinkerSymbolEntry> symbols = new List<LinkerSymbolEntry>();
			foreach (string line in segmentLines)
			{
				Match match = Regex.Match(line, @"\s+(?<name>[^ ]+)\s+(?<start>[a-f0-9]+)", RegexOptions.Compiled);
				if (match.Success)
				{
					symbols.Add(new LinkerSymbolEntry
					{
						_Name = match.Groups["name"].Value,
						_Start = match.Groups["start"].Value
					});
				}
			}

			return symbols;
		}
	}
}
