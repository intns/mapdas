
using System;
using System.Collections.Generic;
using System.Text;

namespace arookas
{

	static class Demangler
	{

		static void DemangleTemplate(StringStream input, StringBuilder output)
		{
			output.Append('<');
			bool end = false;

			do
			{
				DemangleType(input, output);

				switch (input.Read())
				{
					case '>':
						{
							end = true;
							break;
						}
					case ',':
						{
							output.Append(", ");
							break;
						}
					case '\0':
						{
							end = true;
							break;
						}
				}
			} while (!end);

			output.Append('>');
		}

		static void DemangleComponents(IList<ComponentInfo> components, int start, StringBuilder output)
		{
			if (components.Count == 0)
			{
				return;
			}

			ComponentType last = components[start].type;

			while (start < components.Count)
			{
				if (components[start].type != last)
				{
					output.Append(' ');
					last = components[start].type;
				}

				switch (components[start].type)
				{
					case ComponentType.Const: output.Append("const"); break;
					case ComponentType.Pointer: output.Append('*'); break;
					case ComponentType.Reference: output.Append('&'); break;
					case ComponentType.Unsigned: output.Append("unsigned"); break;
					case ComponentType.Ellipsis: output.Append("..."); break;
					case ComponentType.Void: output.Append("void"); break;
					case ComponentType.Bool: output.Append("bool"); break;
					case ComponentType.Char: output.Append("char"); break;
					case ComponentType.WChar: output.Append("wchar_t"); break;
					case ComponentType.Short: output.Append("short"); break;
					case ComponentType.Int: output.Append("int"); break;
					case ComponentType.Long: output.Append("long"); break;
					case ComponentType.LongLong: output.Append("long long"); break;
					case ComponentType.Float: output.Append("float"); break;
					case ComponentType.Double: output.Append("double"); break;
					case ComponentType.Type: output.Append(components[start].name); break;
					case ComponentType.Func:
						{
							output.Append(components[start].name);
							output.Append(' ');

							if ((start + 1) < components.Count)
							{
								output.Append('(');
								DemangleComponents(components, (start + 1), output);
								output.Append(") ");
							}

							output.Append('(');
							output.Append(components[start].prms);
							output.Append(')');
							return;
						}
					case ComponentType.Array:
						{
							int count = 0;

							while ((start + count) < components.Count && components[start + count].type == ComponentType.Array)
							{
								++count;
							}

							if (count > 0 && (start + count) < components.Count)
							{
								output.Append('(');
								DemangleComponents(components, (start + count), output);
								output.Append(") ");
							}

							// output in reverse, e.g. "A4_A3_" becomes "[3][4]"
							while (count-- > 0)
							{
								output.Append('[');
								output.Append(components[start + count].length);
								output.Append(']');
							}
							return;
						}
				}

				++start;
			}
		}

		static void DemangleType(StringStream input, StringBuilder output)
		{
			char c = input.Peek();

			if (c == '-' || ('0' <= c && c <= '9'))
			{
				bool literal = false;
				bool negative = false;

				if (c == '-')
				{
					++input.Position;
					c = input.Peek();

					literal = true;
					negative = true;
				}

				int length = 0;

				while ('0' <= c && c <= '9')
				{
					length *= 10;
					length += (c - '0');

					++input.Position;
					c = input.Peek();
				}

				if (c == ',' || c == '>')
				{
					literal = true;
				}

				if (literal)
				{
					if (negative)
					{
						length = -length;
					}

					output.Append(length.ToString());
				}
				else
				{
					int start = input.Position;

					while ((input.Position - start) < length
						&& input.Position != input.Length)
					{
						c = input.Read();

						if (c == '<')
						{
							DemangleTemplate(input, output);
						}
						else
						{
							output.Append(c);
						}
					}
				}
			}
			else
			{
				bool end = false;
				List<ComponentInfo> components = new List<ComponentInfo>(50);

				do
				{
					c = input.Read();

					if (c == '\0')
					{
						end = true;
					}

					switch (c)
					{
						case 'C':
							{
								components.Insert(0, new ComponentInfo(ComponentType.Const));
								break;
							}
						case 'P':
							{
								components.Insert(0, new ComponentInfo(ComponentType.Pointer));
								break;
							}
						case 'R':
							{
								components.Insert(0, new ComponentInfo(ComponentType.Reference));
								break;
							}
						case 'U':
							{
								components.Insert(0, new ComponentInfo(ComponentType.Unsigned));
								break;
							}
						case 'A':
							{
								int length = 0;

								while ((c = input.Read()) != '_')
								{
									length *= 10;
									length += (c - '0');
								}

								components.Insert(0, new ComponentInfo(length));
								break;
							}
						case 'e':
							{
								components.Insert(0, new ComponentInfo(ComponentType.Ellipsis));
								end = true;
								break;
							}
						case 'v':
							{
								components.Insert(0, new ComponentInfo(ComponentType.Void));
								end = true;
								break;
							}
						case 'b':
							{
								components.Insert(0, new ComponentInfo(ComponentType.Bool));
								end = true;
								break;
							}
						case 'c':
							{
								components.Insert(0, new ComponentInfo(ComponentType.Char));
								end = true;
								break;
							}
						case 'w':
							{
								components.Insert(0, new ComponentInfo(ComponentType.WChar));
								end = true;
								break;
							}
						case 's':
							{
								components.Insert(0, new ComponentInfo(ComponentType.Short));
								end = true;
								break;
							}
						case 'i':
							{
								components.Insert(0, new ComponentInfo(ComponentType.Int));
								end = true;
								break;
							}
						case 'l':
							{
								components.Insert(0, new ComponentInfo(ComponentType.Long));
								end = true;
								break;
							}
						case 'x':
							{
								components.Insert(0, new ComponentInfo(ComponentType.LongLong));
								end = true;
								break;
							}
						case 'f':
							{
								components.Insert(0, new ComponentInfo(ComponentType.Float));
								end = true;
								break;
							}
						case 'd':
							{
								components.Insert(0, new ComponentInfo(ComponentType.Double));
								end = true;
								break;
							}
						case 'Q':
							{
								int count = (input.Read() - '0');
								StringBuilder name = new StringBuilder(500);

								while (count-- > 0)
								{
									DemangleType(input, name);

									if (count > 0)
									{
										name.Append("::");
									}
								}

								components.Insert(0, new ComponentInfo(name.ToString()));
								end = true;
								break;
							}
						case 'F':
							{
								StringBuilder prms = new StringBuilder(500);
								StringBuilder ret = new StringBuilder(500);

								for (char nc = input.Peek(); nc != '_' && nc != '\0'; nc = input.Peek())
								{
									if (prms.Length > 0)
									{
										prms.Append(", ");
									}

									DemangleType(input, prms);
								}

								// i.e. prefer "()" over "(void)"
								string prms_str = prms.ToString();

								if (prms_str == "void")
								{
									prms_str = "";
								}

								++input.Position;
								DemangleType(input, ret);
								components.Insert(0, new ComponentInfo(prms_str, ret.ToString()));
								end = true;
								break;
							}
						default:
							{
								if ('0' <= c && c <= '9')
								{
									--input.Position;
									StringBuilder name = new StringBuilder(500);
									DemangleType(input, name);
									components.Insert(0, new ComponentInfo(name.ToString()));
									end = true;
								}
								break;
							}
					}
				} while (!end);

				// e.g. prefer "unsigned int" over "int unsigned"
				int const_index = 1;
				if (components.Count >= 2 && components[1].type == ComponentType.Unsigned)
				{
					components.RemoveAt(1);
					components.Insert(0, new ComponentInfo(ComponentType.Unsigned));
					++const_index;
				}

				// e.g. prefer "const float *" over "float const *"
				if (components.Count >= (const_index + 1) && components[const_index].type == ComponentType.Const)
				{
					components.RemoveAt(const_index);
					components.Insert(0, new ComponentInfo(ComponentType.Const));
				}

				DemangleComponents(components, 0, output);
			}
		}

		static int ScanNameEnd(StringStream input)
		{
			int end = input.Length;

			for (int i = input.Position; i < input.Length; ++i)
			{
				if (input[i] != '_' || input[i + 1] != '_')
				{
					continue;
				}

				int digit_index = (i + 2);
				if (input[i + 2] == 'Q')
				{
					++digit_index;
				}
				else if (input[i + 2] != 'F')
				{
					char c = input[digit_index];

					if (c < '0' || c > '9')
					{
						continue;
					}
				}

				end = i;
			}

			return end;
		}

		static string DemangleName(StringStream input)
		{
			StringBuilder output = new StringBuilder(500);
			int end = ScanNameEnd(input);

			while (input.Position < end)
			{
				char c = input.Read();

				if (c == '<')
				{
					DemangleTemplate(input, output);
				}
				else
				{
					output.Append(c);
				}
			}

			if (end < input.Length)
			{
				input.Position += 2;
			}

			return output.ToString();
		}

		public static string Demangle(string symbol)
		{
			StringStream input = new StringStream(symbol);
			StringBuilder output = new StringBuilder(1000);

			string name = DemangleName(input);
			string type = null;
			string prms = null;
			bool constant = false;

			if (input.Position < input.Length && input.Peek() != 'F')
			{
				DemangleType(input, output);
				type = output.ToString();
				output.Clear();
			}

			if (input.Peek() == 'C')
			{
				++input.Position;
				constant = true;
			}

			if (input.Peek() == 'F')
			{
				++input.Position;

				while (input.Position < input.Length)
				{
					if (output.Length > 0)
					{
						output.Append(", ");
					}

					DemangleType(input, output);
				}

				prms = output.ToString();
				output.Clear();
			}

			if (type != null)
			{
				output.Append(type);
				output.Append("::");
			}

			if (name != null)
			{
				bool op = true;
				bool space = true;

				switch (name)
				{
					case "__ct":
					case "__dt":
						{
							if (type == null)
							{
								break;
							}

							if (name == "__dt")
							{
								output.Append('~');
							}

							name = type;
							int index = type.IndexOf('<');

							if (index >= 0)
							{
								name = name.Substring(0, index);
							}

							index = name.LastIndexOf("::");

							if (index >= 0)
							{
								name = name.Substring(index + 2);
							}

							op = false;
							space = false;
							break;
						}
					case "__nw": name = " new"; break;
					case "__nwa": name = " new[]"; break;
					case "__dl": name = " delete"; break;
					case "__dla": name = " delete[]"; break;
					case "__pl": name = "+"; break;
					case "__mi": name = "-"; break;
					case "__ml": name = "*"; break;
					case "__dv": name = "/"; break;
					case "__md": name = "%"; break;
					case "__er": name = "^"; break;
					case "__ad": name = "&"; break;
					case "__or": name = "|"; break;
					case "__co": name = "~"; break;
					case "__nt": name = "!"; break;
					case "__as": name = "="; break;
					case "__lt": name = "<"; break;
					case "__gt": name = ">"; break;
					case "__apl": name = "+="; break;
					case "__ami": name = "-="; break;
					case "__amu": name = "*="; break;
					case "__adv": name = "/="; break;
					case "__amd": name = "%="; break;
					case "__aer": name = "^="; break;
					case "__aad": name = "&="; break;
					case "__aor": name = "|="; break;
					case "__ls": name = "<<"; break;
					case "__rs": name = ">>"; break;
					case "__ars": name = ">>="; break;
					case "__als": name = "<<="; break;
					case "__eq": name = "=="; break;
					case "__ne": name = "!="; break;
					case "__le": name = "<="; break;
					case "__ge": name = ">="; break;
					case "__aa": name = "&&"; break;
					case "__oo": name = "||"; break;
					case "__pp": name = "++"; break;
					case "__mm": name = "--"; break;
					case "__cm": name = ","; break;
					case "__rm": name = "->*"; break;
					case "__rf": name = "->"; break;
					case "__cl": name = "()"; break;
					case "__vc": name = "[]"; break;
					default:
						{
							op = false;
							space = false;
							break;
						}
				}

				if (op)
				{
					output.Append("operator");
				}

				output.Append(name);

				if (space)
				{
					output.Append(' ');
				}
			}

			if (prms != null)
			{
				output.Append('(');

				if (prms != "void")
				{
					output.Append(prms);
				}

				output.Append(')');
			}

			if (constant)
			{
				output.Append(" const");
			}

			return output.ToString();
		}

		enum ComponentType
		{

			Const,
			Pointer,
			Reference,
			Unsigned,
			Ellipsis,
			Void,
			Bool,
			Char,
			WChar,
			Short,
			Int,
			Long,
			LongLong,
			Float,
			Double,
			Type,
			Func,
			Array,

		}

		struct ComponentInfo
		{

			public ComponentType type;
			public int length; // array dimension length
			public string name; // user type or return type
			public string prms; // parameter types

			public ComponentInfo(ComponentType type) : this(type, 0, "", "") { }
			public ComponentInfo(int length) : this(ComponentType.Array, length, "", "") { }
			public ComponentInfo(string name) : this(ComponentType.Type, 0, name, "") { }
			public ComponentInfo(string prms, string ret) : this(ComponentType.Func, 0, ret, prms) { }
			public ComponentInfo(ComponentType type, int length, string name, string prms)
			{
				this.type = type;
				this.length = length;
				this.name = name;
				this.prms = prms;
			}

		}

		public class StringStream
		{

			public string Data { get; private set; }
			public int Position { get; set; }
			public int Length { get { return Data.Length; } }

			public char this[int index]
			{
				get
				{
					if (index >= Length)
					{
						return '\0';
					}

					return Data[index];
				}
			}

			public StringStream(string data)
			{
				Data = data;
				Position = 0;
			}

			public char Read()
			{
				if (Position >= Length)
				{
					return '\0';
				}

				return Data[Position++];
			}

			public char Peek()
			{
				if (Position >= Length)
				{
					return '\0';
				}

				return Data[Position];
			}

			public string Read(int count)
			{
				int read = Math.Min(count, (Length - Position));

				if (read <= 0)
				{
					return new String('\0', count);
				}

				int fill = (count - read);
				string data = Data.Substring(Position, read);
				Position += read;

				if (fill > 0)
				{
					data += new String('\0', fill);
				}

				return data;
			}

			public string Peek(int count)
			{
				int read = Math.Min(count, (Length - Position));

				if (read <= 0)
				{
					return new String('\0', count);
				}

				int fill = (count - read);
				string data = Data.Substring(Position, read);

				if (fill > 0)
				{
					data += new String('\0', fill);
				}

				return data;
			}

		}

	}

}