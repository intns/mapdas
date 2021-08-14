using System.Collections.Generic;
using static arookas.Demangler;

namespace mapdas
{
	public static class SYMParsing
	{
		// sy must be a demangled symbol (includes parameter arguments like "function(char*, int)")
		public static List<string> GetTypes(string sy)
		{
			int openBracketIdx = sy.IndexOf("(");
			if (openBracketIdx == -1)
			{
				return null;
			}

			if (sy.Contains("operator()"))
			{
				sy = sy.Replace("operator()", "");
			}

			List<string> parameters = new List<string>();
			string functionName = string.Empty;
			SeperateTypes(sy, ref parameters, ref functionName);
			return parameters;
		}

		// seperates a comma-seperated list of types, with special handling for function pointers
		public static void SeperateTypes(string s, ref List<string> parameters, ref string functionName)
		{
			StringStream input = new StringStream(s);
			int level = 0;
			int pIdx = 0;
			int prevPIdx = -1;
			while (input.Position < input.Length)
			{
				char r = input.Read();

				switch (r)
				{
					case '(':
						level++;
						break;
					case '<':
						if (level != 0)
						{
							level++;
						}
						break;
				}

				if (level == 0)
				{
					functionName += r;
				}
				else
				{
					if (prevPIdx != pIdx)
					{
						parameters.Add(r.ToString());
						prevPIdx = pIdx;
					}
					else
					{
						parameters[pIdx] += r;
					}
				}

				switch (r)
				{
					case ')':
						level--;
						break;
					case '>':
						if (level != 0)
						{
							level--;
						}
						break;
					case ',':
						if (level == 1)
						{
							pIdx++;
						}
						break;
				}
			}

			if (parameters.Count > 0)
			{
				parameters[0] = parameters[0].Remove(0, 1);
				parameters[parameters.Count - 1] = parameters[parameters.Count - 1].Remove(parameters[parameters.Count - 1].Length - 1, 1);

				for (int i = 0; i < parameters.Count; i++)
				{
					parameters[i] = parameters[i].Trim().TrimEnd(',');

					if (string.IsNullOrEmpty(parameters[i]))
					{
						parameters.RemoveAt(i);
						i--;
					}
				}
			}
		}
	}
}
