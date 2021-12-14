using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace CommonDecompFiller
{
	static class Filler
	{
		static private void EncapsulateNamespace(string name, ref List<string> lines)
		{
			bool insideNamespace = false;
			for (int i = 0; i < lines.Count; i++)
			{
				string line = lines[i].Trim();
				bool checkCtor = line.StartsWith(name + "::") && line.Contains("(");
				// Only support u32, s32 and void functions as we can
				// be sure these are atleast automatically generated
				if (checkCtor ||
					((line.StartsWith("void") || line.StartsWith("u32") || line.StartsWith("s32"))
											&& line.Contains("(")))
				{
					// Fix double parenthesis
					if (line.Contains("(("))
					{
						// Replace the ((
						lines[i] = lines[i].Replace("((", "(");
						if (line.Contains("))"))
						{
							lines[i] = lines[i].Replace("))", ")");
						}
						else
						{
							// Search the next 5 lines for the ending ))
							for (int j = 0; j < 10; j++)
							{
								if (lines[i + j].Contains("))"))
								{
									lines[i + j] = lines[i + j].Replace("))", ")");
									break;
								}
							}
						}
					}

					string type = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries).Where(s => !string.IsNullOrWhiteSpace(line)).ToArray()[0];
					if (!checkCtor)
					{
						// Check if we are actually in the namespace
						line = line.Remove(0, type.Length).Trim();
					}

					if (checkCtor || line.StartsWith(name + "::"))
					{
						if (insideNamespace == false)
						{
							lines.Insert(i - 5, $"namespace {name} {{\n");
							i++;
							insideNamespace = true;
						}

						// Remove the name and the :: afterwards
						lines[i] = line.Remove(0, name.Length + 2);
						if (!checkCtor)
						{
							lines[i] = lines[i].Insert(0, type + " ");
						}
					}
					else if (insideNamespace)
					{
						lines.Insert(i - 5, $"}} // {name}\n");
						i++;
						insideNamespace = false;
					}
				}
			}

			if (insideNamespace)
			{
				lines.Add($"}} // {name}\n");
			}
		}

		static private void HandleSimpleStorePattern(string version, int i, ref List<string> lines)
		{
			if (i + 3 < lines.Count
	&& lines[i + 1].Contains("li")
	&& lines[i + 2].Contains(version)
	&& lines[i + 3].Contains("blr"))
			{
				if (lines[i - 3].StartsWith("void"))
				{
					// Check if it's in a class or not
					string[] scopeList = lines[i - 3].Remove(0, 5).Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
					if (scopeList.Length >= 1)
					{
						string liLine = lines[i + 1];
						string[] liTokens = liLine.Split();
						for (int j = 0; j < liTokens.Length; j++)
						{
							liTokens[j] = liTokens[j].Trim();
						}
						var liFinalTokens = liTokens.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

						if (liFinalTokens[0] == "li" && liFinalTokens[1] == "r0,")
						{
							bool liValueNegative = liFinalTokens[2].Contains("-");
							string liValueHex = liFinalTokens[2].Replace("-", "");
							int liValue = 0;
							if (liValueHex.StartsWith("0x"))
							{
								liValue = int.Parse(liValueHex.Replace("0x", ""), NumberStyles.HexNumber);
								if (liValueNegative)
								{
									liValue = -liValue;
								}
							}
							else
							{
								liValue = int.Parse(liValueHex);
							}

							string stwLine = lines[i + 2];
							string[] stwTokens = stwLine.Split();
							for (int j = 0; j < stwTokens.Length; j++)
							{
								stwTokens[j] = stwTokens[j].Trim();
							}
							var stwFinalTokens = stwTokens.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

							if (stwFinalTokens[0] == version && stwFinalTokens[1] == "r0,"
								&& stwFinalTokens[2].EndsWith("(r3)"))
							{
								string stwValueHex = stwFinalTokens[2].Replace("0x", "").Replace("(r3)", "");
								int stwValueSet = int.Parse(stwValueHex, NumberStyles.HexNumber);

								lines.RemoveAt(i - 1);
								lines.RemoveAt(i - 1);
								lines.RemoveAt(i - 1);
								lines.RemoveAt(i - 1);
								lines.RemoveAt(i - 1);
								lines.RemoveAt(i - 1);

								lines.Insert(i - 1, $"_{stwValueSet.ToString("X2").ToUpper()} = {liValue};");
								lines.Insert(i - 1, $"// Generated from {version} r0, 0x{stwValueHex}(r3)");
							}
						}
					}
				}
			}

		}

		static private void HandleSimpleArgStorePattern(string version, int i, ref List<string> lines)
		{
			if (i + 2 < lines.Count
								&& lines[i + 1].Contains(version)
								&& lines[i + 2].Contains("blr"))
			{
				if (lines[i - 3].StartsWith("void"))
				{
					// Check if it's in a class or not
					string[] scopeList = lines[i - 3].Remove(0, 5).Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
					if (scopeList.Length >= 1)
					{
						string stwLine = lines[i + 1];
						string[] stwTokens = stwLine.Split();
						for (int j = 0; j < stwTokens.Length; j++)
						{
							stwTokens[j] = stwTokens[j].Trim();
						}
						var stwFinalTokens = stwTokens.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

						if (stwFinalTokens[0] == version && stwFinalTokens[1] == "r4,"
							&& stwFinalTokens[2].EndsWith("(r3)"))
						{
							string stwValueHex = stwFinalTokens[2].Replace("0x", "").Replace("(r3)", "");
							int stwValueSet = int.Parse(stwValueHex, NumberStyles.HexNumber);

							lines.RemoveAt(i - 1);
							lines.RemoveAt(i - 1);
							lines.RemoveAt(i - 1);
							lines.RemoveAt(i - 1);
							lines.RemoveAt(i - 1);

							// Set the variable name in the arguments
							int lastBracket = lines[i - 3].LastIndexOf(")");
							if (lastBracket != -1)
							{
								lines[i - 3] = lines[i - 3].Insert(lastBracket, " a1");

								lines.Insert(i - 1, $"_{stwValueSet.ToString("X2").ToUpper()} = a1;");
								lines.Insert(i - 1, $"// Generated from {version} r4, 0x{stwValueHex}(r3)");
							}
						}
					}
				}
			}
		}

		static public void Run(string src, List<string> namespaces)
		{
			List<string> files = new List<string>();
			files.AddRange(Directory.GetFiles(src, "*.cpp", SearchOption.AllDirectories));
			files.AddRange(Directory.GetFiles(src, "*.c", SearchOption.AllDirectories));

			foreach (string file in files)
			{
				List<string> lines = File.ReadAllLines(file).ToList();

				for (int i = 0; i < lines.Count; i++)
				{
					if (lines[i].StartsWith("void"))
					{
						string[] seperators = new string[] { "::", " " };
						int end = lines[i].IndexOf("(");
						if (end != -1)
						{
							string[] scopeSplit = lines[i].Substring(0, end).Split(seperators, StringSplitOptions.RemoveEmptyEntries);
							if (scopeSplit.Length > 1
								&& (scopeSplit[scopeSplit.Length - 1] == scopeSplit[scopeSplit.Length - 2]
								|| scopeSplit[scopeSplit.Length - 1] == "~" + scopeSplit[scopeSplit.Length - 2]))
							{
								lines[i] = lines[i].Remove(0, 5);
							}
						}
					}

					if (lines[i].Contains("__ct"))
					{
						int ctIdx = lines[i].LastIndexOf("__ct");
						string cutOff = lines[i].Substring(0, ctIdx).Trim();
						string[] splitByScope = cutOff.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
						if (splitByScope.Length > 2)
						{
							string ctorName = splitByScope[splitByScope.Length - 2];
							lines[i] = lines[i].Replace("__ct", ctorName);
							if (lines[i].StartsWith("void"))
							{
								lines[i] = lines[i].Remove(0, 4).Trim();
							}
						}
					}
					else if (lines[i].Contains("__dt"))
					{
						int dtIdx = lines[i].LastIndexOf("__dt");
						string cutOff = lines[i].Substring(0, dtIdx).Trim();
						string[] splitByScope = cutOff.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries);
						if (splitByScope.Length > 2)
						{
							string dtorName = "~" + splitByScope[splitByScope.Length - 2];
							lines[i] = lines[i].Replace("__dt", dtorName);

							if (lines[i].StartsWith("void"))
							{
								lines[i] = lines[i].Remove(0, 4).Trim();
							}
						}
					}
					else if (lines[i].Contains("( const") && !lines[i].Contains("( const("))
					{
						lines[i] = lines[i].Replace("( const", "() const");
					}
					else if (lines[i].Contains("void _Print(char*, ...)"))
					{
						lines.RemoveAt(i);
						lines.RemoveAt(i);
						lines.RemoveAt(i);
						lines.RemoveAt(i);
						lines.RemoveAt(i - 1);
						lines.RemoveAt(i - 2);
						lines.RemoveAt(i - 3);
						lines.RemoveAt(i - 4);
						lines.RemoveAt(i - 5);
					}
					// Codegen pattern recognition
					else if (lines[i].Contains(".loc_0x0:"))
					{
						// Handle copying from the .s to the function

						if (i + 1 != lines.Count
							&& lines[i + 1].Contains("blr"))
						{
							lines.RemoveAt(i - 1);
							lines.RemoveAt(i - 1);
							lines.RemoveAt(i - 1);
							lines.RemoveAt(i - 1);
						}
						else if (i + 2 < lines.Count
							&& lines[i + 1].Contains("li")
							&& lines[i + 2].Contains("blr"))
						{
							string liLine = lines[i + 1];
							string[] tokens = liLine.Split();
							for (int j = 0; j < tokens.Length; j++)
							{
								tokens[j] = tokens[j].Trim();
							}

							var filteredTokens = tokens.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();

							if (filteredTokens[0] == "li" && filteredTokens[1] == "r3,")
							{
								string numHex = filteredTokens[2].Replace("0x", "");
								bool negative = numHex.StartsWith("-");

								int num = int.Parse(negative ? numHex.Remove(0, 1) : numHex, System.Globalization.NumberStyles.HexNumber);
								lines.RemoveAt(i - 1);
								lines.RemoveAt(i - 1);
								lines.RemoveAt(i - 1);
								lines.RemoveAt(i - 1);
								lines.RemoveAt(i - 1);
								string retLine = "\treturn ";
								if (negative)
								{
									retLine += "-";
								}
								retLine += "0x" + num.ToString("X") + ";";
								lines.Insert(i - 1, retLine);

								if (lines[i - 3].StartsWith("void"))
								{
									string oldFunc = lines[i - 3];
									oldFunc = oldFunc.Remove(0, 4);
									oldFunc = oldFunc.Insert(0, negative ? "s32 " : "u32 ");
									lines[i - 3] = oldFunc;
								}
							}
						}

						HandleSimpleStorePattern("stw", i, ref lines);
						HandleSimpleStorePattern("sth", i, ref lines);
						HandleSimpleStorePattern("stb", i, ref lines);

						HandleSimpleArgStorePattern("stw", i, ref lines);
						HandleSimpleArgStorePattern("sth", i, ref lines);
						HandleSimpleArgStorePattern("stb", i, ref lines);
					}
				}

				if (file.EndsWith(".cpp"))
				{
					// Handle namespacing the C++ code
					for (int i = 0; i < namespaces.Count; i++)
					{
						EncapsulateNamespace(namespaces[i], ref lines);
					}
				}

				File.WriteAllLines(file, lines.ToArray());
			}
		}
	}
}
