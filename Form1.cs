using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static arookas.Demangler;

namespace mapdas
{
	public partial class Form1 : Form
	{
		private readonly List<MAP> _FilesOpen = new List<MAP>();
		private int _SelectedFile = 0;

		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			/* IMPORTANT
       * the MapTree ViewTree will give "tags" to the different levels of information
       * displayed on the screen at any one time with a symbol map loaded, it will go
       * as follows:
       *
       * Files (e.g. "G:/Hacking/mapdas/pikmin-1.map") will be tagged "File" (image list index 0)
       * Paths (e.g. "jaudio/bankdrv.c") will be tagged "Path" (image list index 1)
       * Functions (e.g. "Bank_RandToOfs(Rand_ *)") will be tagged "Function" (image list index 2)
       */
			_FileContextMenu.MenuItems[0].Click += OpenDol_Click; // OpenDol
			_FileContextMenu.MenuItems[1].Click += ExportFilesystem_Click; // ExportFilesystem
			_FileContextMenu.MenuItems[2].Click += ExportToLinkerMap_Click; // ExportToLinkerMap
			_FileContextMenu.MenuItems[3].Click += ConvertToIdc_Click; // ConvertToIdc
			_FileContextMenu.MenuItems[4].Click += ExportTypesMap_Click; // ExportTypes
			_FileContextMenu.MenuItems[5].Click += CloseMap_Click; // CloseMap

			_PathContextMenu.MenuItems[0].Click += ExportFile_Click; // ExportFile
			_PathContextMenu.MenuItems[1].Click += ExportTypesFile_Click; // ExportTypes

			_FunctionContextMenu.MenuItems[0].Click += ExportTypesFunction_Click; // ExportTypes
			_FunctionContextMenu.MenuItems[1].Click += DecompileToView_Click; // DecompileToView
		}

		// Functionality on right click
		private readonly ContextMenu _FileContextMenu = new ContextMenu(new MenuItem[] {
			new MenuItem("Open DOL"),
			new MenuItem("Export Filesystem"),
			new MenuItem("Export To Linker Map"),
			new MenuItem("Convert To IDC"),
			new MenuItem("Export Types"),
			new MenuItem("Close")
		});
		private readonly ContextMenu _PathContextMenu = new ContextMenu(new MenuItem[] {
			new MenuItem("Export File"),
			new MenuItem("Export Types")
		});
		private readonly ContextMenu _FunctionContextMenu = new ContextMenu(new MenuItem[] {
			new MenuItem("Export Types"),
			new MenuItem("Decompile")
		});
		private void MapTree_Click(object sender, EventArgs ea)
		{
			MouseEventArgs e = (MouseEventArgs)ea;

			TreeNode node = MapTree.GetNodeAt(e.X, e.Y);
			MapTree.SelectedNode = node;
			string nodeTag = (string)node.Tag;

			switch (nodeTag)
			{
				case "File":
					_SelectedFile = node.Index;
					break;
				case "Path":
					_SelectedFile = node.Parent.Index;
					break;
				case "Function":
					_SelectedFile = node.Parent.Parent.Index;
					break;
				default:
					break;
			}

			if (e.Button != MouseButtons.Right)
			{
				return;
			}

			switch (nodeTag)
			{
				case "File":
					_FileContextMenu.Show(MapTree, new Point(e.X, e.Y));
					break;
				case "Path":
					_PathContextMenu.Show(MapTree, new Point(e.X, e.Y));
					break;
				case "Function":
					_FunctionContextMenu.Show(MapTree, new Point(e.X, e.Y));
					break;
				default:
					break;
			}
		}

		// TargetPath is the path in the symbol map
		// OutputPath is the path where we dump everything
		private void ExportFile(MAP file, string targetPath, string outputPath)
		{
			foreach (MAPParsing.TextEntry node in file._TextSymbols.FindAll(x => x._Path == targetPath))
			{
				if (node._Address.Contains("..."))
				{
					continue;
				}

				string function = node._SymbolDemangled;

				if (!function.Contains("("))
				{
					function += "(void)";
				}

				string set = $"\n\n/*\n * --INFO--\n * Address:\t{node._Address.ToUpper()}\n * Size:\t{node._Size.ToUpper()}\n */\nvoid {function}\n{{\n";

				if (file._AssociatedDol != null)
				{
					// we have a DOL file we can read PPC from
					try
					{
						set += "/*\n" + ReadAddressFromDol(file._AssociatedDol, node._Address, node._Size) + "\n*/\n";
					}
					catch (Exception)
					{
						// Failed to read from address
						set += "\t// TODO\n";
					}
				}
				else
				{
					// no PPC :(
					set += "\t// TODO\n";
				}

				set += "}";

				// Normalize line endings
				set = Regex.Replace(set, @"\r\n|\n\r|\n|\r", "\r\n");
				File.AppendAllText(outputPath, set);
				Update();
			}
		}

		private void ExportFile_Click(object o, EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog
			{
				Filter = "Any Files (*.*)|*.*|C++ Files (*.cpp)|*.cpp|C Files (*.c)|*.c",
				FileName = Path.GetFileName(MapTree.SelectedNode.Text)
			};

			if (dialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			ExportFile(_FilesOpen[_SelectedFile], MapTree.SelectedNode.Text, dialog.FileName);
		}

		private void ExportFilesystem_Click(object o, EventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
			};

			if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
			{
				return;
			}

			foreach (TreeNode node in MapTree.SelectedNode.Nodes)
			{
				if (!Directory.Exists(dialog.FileName + "/" + Path.GetDirectoryName(node.Text)))
				{
					Directory.CreateDirectory(dialog.FileName + "/" + Path.GetDirectoryName(node.Text.Replace(":", "")));
				}

				ExportFile(_FilesOpen[_SelectedFile], node.Text, dialog.FileName + "/" + node.Text.Replace(":", ""));
				DebugLog.SelectionStart = DebugLog.Text.Length;
				DebugLog.ScrollToCaret();

				Update();
				Application.DoEvents();
			}

			DebugLog.Text += "Done!\n";
		}

		private void ExportToLinkerMap_Click(object o, EventArgs e)
		{
			SaveFileDialog diag = new SaveFileDialog()
			{
				Filter = "Linker Map (*.txt)|*.txt",
				FileName = "linker-map.txt",
			};

			if (diag.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			using (StreamWriter writer = new StreamWriter(diag.FileName))
			{
				MAP openMap = _FilesOpen[_SelectedFile];
				foreach (MAPParsing.TextEntry entry in openMap._TextSymbols)
				{
					if (entry._Address.Contains("."))
					{
						continue;
					}

					writer.WriteLine($"{entry._Symbol}=0x{entry._Address.ToUpper()}");
				}
			}
		}


		private void CloseMap_Click(object o, EventArgs e)
		{
			DebugLog.Text += "Closing Symbol Map... ";
			Update();
			MapTree.BeginUpdate();

			MapTree.Nodes.Remove(MapTree.SelectedNode);
			_FilesOpen.RemoveAt(_SelectedFile);
			_SelectedFile = 0;

			MapTree.EndUpdate();
			DebugLog.Text += "Done!\n";
			GC.Collect();
		}

		private void ConvertToIdc_Click(object o, EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog
			{
				Filter = "IDC File (*.idc)|*.idc",
				RestoreDirectory = true
			};

			if (dialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			DebugLog.Text += $"Saving IDC to {dialog.FileName}... ";

			using (StreamWriter sw = new StreamWriter(File.OpenWrite(dialog.FileName)))
			{
				sw.WriteLine("#include <idc.idc>\n");

				sw.WriteLine("static main()");
				sw.WriteLine("{");
				sw.WriteLine("\tset_inf_attr(INF_GENFLAGS, INFFL_LOADIDC|get_inf_attr(INF_GENFLAGS));\n");
				sw.WriteLine("\thandleSegments();");
				sw.WriteLine("\thandleFunctions();");
				sw.WriteLine("}\n");

				// SEGMENTS
				sw.WriteLine("static handleSegments()");
				sw.WriteLine("{");

				List<string> segments = new List<string>();
				foreach (MAPParsing.MemoryMapEntry symbol in MAPParsing.ParseMemoryMapSection(_FilesOpen[_SelectedFile]))
				{
					sw.WriteLine($"\tSegRename(0x{symbol._Start}, \"{symbol._Name}\");");
					segments.Add(symbol._Name);
				}

				foreach (MAPParsing.LinkerSymbolEntry symbol in MAPParsing.ParseLinkerSymbolSection(_FilesOpen[_SelectedFile]))
				{
					sw.WriteLine($"\tset_name(0x{symbol._Start}, \"{symbol._Name}\");");
				}
				sw.WriteLine("}\n");

				// WRITING FUNCTIONS
				sw.WriteLine("static handleFunctions()");
				sw.WriteLine("{");
				foreach (string[] lines in from string segment in segments
																	 let lines = _FilesOpen[_SelectedFile].TryGetSection($"{segment} section layout")
																	 select lines)
				{
					if (lines == null)
					{
						continue;
					}

					foreach (string line in lines)
					{
						Match match = Regex.Match(line, @"\s+(?<start>[a-f0-9]+)\s+(?<size>[a-f0-9]+)\s+(?<address>[a-f0-9]+)\s+(?<type>\d+)\s+(?<name>[^ ]+)\s*(?<module>.+)?", RegexOptions.Compiled);
						if (!match.Success)
						{
							continue;
						}

						string name = match.Groups["name"].Value.Trim();

						if (name == ".text")
						{
							continue;
						}

						// rename items that are strings or un-named globals (const strings for instance)
						if (name.StartsWith("@") && match.Groups["module"].Success)
						{
							string moduleCppFile = match.Groups["module"].Value;

							for (int i = 1; i < moduleCppFile.Length; i++)
							{
								// See if the string has a `.a` in it
								if (moduleCppFile[i] != 'a' || moduleCppFile[i - 1] != '.')
								{
									continue;
								}

								// Remove the bottom half of the string including space/tab
								moduleCppFile = moduleCppFile.Remove(0, i + 2) + "_";
								break;
							}

							// Insert the cppFile before the number
							name = moduleCppFile + name.Replace("@", "");
						}

						if (match.Groups["type"].Value == "4" || match.Groups["type"].Value == "32")
						{
							// rename items that are the same name as essential PPC registers (r0 - r31, f0 - f31)
							if (name.Length <= 3 && int.TryParse(name.Substring(1, name.Length - 1), out int r)
								&& (name.StartsWith("r") || name.StartsWith("f")))
							{
								for (int i = 0; i < 31; i++)
								{
									string v = "r" + i.ToString();
									string f = "f" + i.ToString();
									if (name == v || name == f)
									{
										name.Insert(0, "_");
									}
								}
							}

							// Make comments stating which .o file and the file
							if (match.Groups["module"].Success)
							{
								sw.WriteLine("\tMakeComm(0x{0}, \"{1}\");", match.Groups["address"].Value, match.Groups["module"].Value.Trim());
							}

							if (match.Groups["size"].Success)
							{
								// Auto-set vtables
								if (name.StartsWith("__vt__"))
								{
									// Get the amount of elements in the vtbl
									int size = int.Parse(match.Groups["size"].Value, NumberStyles.HexNumber) >> 2;
									int address = int.Parse(match.Groups["address"].Value, NumberStyles.HexNumber);
									for (int i = 0; i < size; i++)
									{
										sw.WriteLine("\tcreate_dword({0});", string.Format("0x{0:X}", address + (i * 4)));
									}
								}
							}
						}
						// Handle floats/doubles
						else if (match.Groups["type"].Value == "8")
						{
							sw.WriteLine("\tcreate_double(0x{0});", match.Groups["address"].Value);
						}
						// Handle C functions
						else if (match.Groups["type"].Value == "16")
						{
							sw.WriteLine("\tSetType(0x{0}, \"void __cdecl {1}\");", match.Groups["address"].Value, name);
						}

						sw.WriteLine("\tset_name(0x{0}, \"{1}\");", match.Groups["address"].Value, name);
					}
				}

				sw.WriteLine("}\n");
			}

			DebugLog.Text += "Done!\n";
		}

		// Function Version (e.g. "test_function(yeah, yeah2)")
		private void ExportTypesFunction_Click(object o, EventArgs e)
		{
			string typeText = string.Empty;

			List<string> types = SYMParsing.GetTypes(MapTree.SelectedNode.Text);
			if (types != null && types.Count > 0)
			{
				foreach (string trimmed in from string parameter in types.Distinct()
																	 let trimmed = parameter.Trim()
																	 where !string.IsNullOrWhiteSpace(trimmed)
																	 select trimmed)
				{
					typeText += $"{trimmed}\n";
				}
			}
			else
			{
				MessageBox.Show("No types found");
				return;
			}

			List<string> sorted = typeText.TrimEnd().Split('\n').Distinct().ToList();
			sorted.Sort();

			Form3 disasmView = new Form3(string.Join("\r\n", sorted));
			disasmView.Show();
		}

		// File Version (e.g. "test_file.cpp")
		private void ExportTypesFile_Click(object o, EventArgs e)
		{
			string typeText = string.Empty;

			foreach (TreeNode function in MapTree.SelectedNode.Nodes)
			{
				List<string> types = SYMParsing.GetTypes(function.Text);
				if (types != null && types.Count > 0)
				{
					foreach (string trimmed in from string parameter in types.Distinct()
																		 let trimmed = parameter.Trim()
																		 where !string.IsNullOrWhiteSpace(trimmed)
																		 select trimmed)
					{
						typeText += $"{trimmed}\n";
					}
				}
			}

			if (string.IsNullOrWhiteSpace(typeText))
			{
				MessageBox.Show("File had no function parameters");
				return;
			}

			List<string> sorted = typeText.TrimEnd().Split('\n').Distinct().ToList();
			sorted.Sort();

			Form3 disasmView = new Form3(string.Join("\r\n", sorted));
			disasmView.Show();
		}

		// Whole symbol map version (e.g. "pikmin-1.map")
		private void ExportTypesMap_Click(object o, EventArgs e)
		{
			string typeText = string.Empty;
			List<string> typeList = new List<string>();
			foreach (List<string> types in from TreeNode file in MapTree.SelectedNode.Nodes
																		 from TreeNode function in file.Nodes
																		 let types = SYMParsing.GetTypes(function.Text)
																		 select types)
			{
				if (types == null || types.Count == 0)
				{
					continue;
				}

				foreach (string trimmed in types)
				{
					/*
           * EXPERIMENTAL
           */
					string[] ignoredTypes = new string[]
						{
									"char",
									"bool",
									"wchar_t",
									"unsigned",
									"signed",
									"short",
									"int",
									"long",
									"float",
									"double",
									"void",
									"...",
						};

					string typeOnly = trimmed.Replace("const", "").Replace("&", "").Replace("*", "").Trim();

					bool skipType = false;
					foreach (string ignored in ignoredTypes)
					{
						if (typeOnly.StartsWith(ignored) || typeOnly == ignored)
						{
							skipType = true;
							break;
						}
					}

					if (skipType || typeOnly.Contains("(") || typeOnly.Contains(")"))
					{
						continue;
					}

					typeList.Add(typeOnly);
				}
			}

			List<string> finalList = new List<string>();
			foreach (string type in typeList)
			{
				string item = string.Empty;
				if (type.Contains("<"))
				{
					item += "template <";
					StringStream input = new StringStream(type);

					// Storing the positions of the open and close "<>" brackets
					int openT = 0;
					int closeT = 0;

					int level = 0;
					int templateTypeCount = 0;
					while (input.Position < input.Length)
					{
						char r = input.Read();

						switch (r)
						{
							case '(':
								level++;
								break;
							case '<':
								if (level == 0)
								{
									level++;
									openT = input.Position;
									templateTypeCount++;
								}
								break;
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
									templateTypeCount++;
								}
								break;
						}
					}

					for (int i = 0; i < templateTypeCount; i++)
					{
						item += $"typename T{i}{(i == templateTypeCount - 1 ? "" : ", ")}";
					}

					item += ">\r\n";
					closeT = type.LastIndexOf('>') + 1;
					item += $"struct {type.Remove(openT - 1, closeT - openT + 1)};";
				}
				else
				{
					item += $"struct {type};";
				}

				item = item.Replace('@', '_');
				item = item.Replace(':', '_');
				item = item.Replace('$', '_');
				finalList.Add(item);
			}

			finalList.Sort();
			Form3 disasmView = new Form3(string.Join("\r\n", finalList.Distinct()));
			disasmView.Show();

			GC.Collect();
		}

		private byte[] ExtractHexFromDol(DOL file, long addr, long size)
		{
			file.Seek(addr, SeekOrigin.Begin);
			byte[] function = file.Read(size);
			return function;
		}

		private void DecompileToView_Click(object o, EventArgs e)
		{
			if (_FilesOpen[_SelectedFile]._AssociatedDol == null)
			{
				MessageBox.Show("Open the DOL file first!");
				return;
			}

			MAPParsing.TextEntry entry = _FilesOpen[_SelectedFile]._TextSymbols.Find(x => x._SymbolDemangled == MapTree.SelectedNode.Text);

			if (entry._Address.Contains("..."))
			{
				MessageBox.Show("The selected function is unused, there is no PPC that can be decompiled");
				return;
			}

			long address = long.Parse(entry._Address, NumberStyles.HexNumber);
			long size = long.Parse(entry._Size, NumberStyles.HexNumber);
			byte[] extractedHex = ExtractHexFromDol(_FilesOpen[_SelectedFile]._AssociatedDol, address, size);
			string[] ppcList = ReadAddressFromDol(_FilesOpen[_SelectedFile]._AssociatedDol, entry._Address, entry._Size).Split('\n');
			string[] hexList = new string[extractedHex.Length >> 2];

			string rawHex = string.Empty;
			int hIndex = 0;
			foreach (byte b in extractedHex)
			{
				rawHex += $"{b:X2}";
				hIndex += 1;
				if (hIndex % 4 == 0)
				{
					hexList[(hIndex - 1) >> 2] = rawHex;
					rawHex = string.Empty;
				}
			}

			string hexViewText = string.Empty;
			long ofs = 0;
			for (int i = 0; i < ppcList.Length; ++i)
			{
				if (ppcList[i].Trim() == "" || ppcList[i].Trim().StartsWith("."))
				{
					hexViewText += Environment.NewLine;
				}
				else
				{
					hexViewText += hexList[ofs] + Environment.NewLine;
					ofs += 1;
				}
			}

			string ppcViewText = ReadAddressFromDol(_FilesOpen[_SelectedFile]._AssociatedDol, entry._Address, entry._Size);

			DisasmView disasmView = new DisasmView(entry._SymbolDemangled, hexViewText, ppcViewText);
			disasmView.Show();
		}

		private string ReadAddressFromDol(DOL file, string address, string size)
		{
			long addr = long.Parse(address, NumberStyles.HexNumber);
			long sz = long.Parse(size, NumberStyles.HexNumber);
			return ReadAddressFromDol(file, addr, sz);
		}

		private string ReadAddressFromDol(DOL file, long address, long size)
		{
			string functionHex = string.Empty;
			byte[] function = ExtractHexFromDol(file, address, size);
			foreach (byte b in function)
			{
				functionHex += $"{b:x2}";
			}

			File.WriteAllText("raw.txt", functionHex);

			using (Process p = new Process())
			{
				p.StartInfo.FileName = "pyiiasmh.exe";
				p.StartInfo.Arguments = $"\"raw.txt\" d --codetype RAW --dest code.txt";
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.CreateNoWindow = true;
				p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
				try
				{
					p.Start();
					p.WaitForExit();
				}
				catch (Exception ex)
				{
					if (ex.Message.ToLower().Contains("not found"))
					{
						MessageBox.Show("Install pyiiasmh before using this function!");
						return "Unable to read\n";
					}

					MessageBox.Show(ex.Message);
					return "Unable to read\n";
				}
			}

			string ppc = string.Empty;
			if (File.Exists("code.txt"))
			{
				ppc = File.ReadAllText("code.txt");
			}

			return ppc.Replace("-----------------", "").Trim();
		}

		private void OpenDol_Click(object sender, EventArgs e)
		{
			if (_FilesOpen[_SelectedFile]._AssociatedDol != null)
			{
				DialogResult result = MessageBox.Show("Symbol map already has an associated DOL file!\nOpen new DOL file anyways?", "Open new DOL file", MessageBoxButtons.YesNo);
				if (result != DialogResult.Yes)
				{
					return;
				}
			}

			OpenFileDialog dialog = new OpenFileDialog()
			{
				CheckFileExists = true,
				CheckPathExists = true,
				ValidateNames = true,

				Filter = "GC/Wii Executable (*.dol)|*.dol"
			};

			if (dialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			DebugLog.Text += "Reading DOL file... ";
			BigEndianReader reader = new BigEndianReader(File.OpenRead(dialog.FileName));
			try
			{
				_FilesOpen[_SelectedFile]._AssociatedDol = new DOL(reader);
			}
			catch (Exception any)
			{
				string errorMessage = $"Error: {any.Message}";

				MessageBox.Show(errorMessage);
				DebugLog.Text += errorMessage + "\n";
			}
			DebugLog.Text += "Done!\n";
		}

		private void FileToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog()
			{
				CheckFileExists = true,
				CheckPathExists = true,
				ValidateNames = true,

				Filter = "Symbol Map (*.map)|*.map"
			};

			if (dialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			// Checking if the symbol map is already open
			foreach (TreeNode files in MapTree.Nodes)
			{
				if (dialog.FileName == files.Text)
				{
					DialogResult result = MessageBox.Show("This symbol map has already been loaded,\nare you sure you want to add the same one?", "Symbol map already open", MessageBoxButtons.YesNo);
					if (result != DialogResult.Yes)
					{
						return;
					}
				}
			}

			DebugLog.Text += "Opening Map File... ";
			MAP newFile = new MAP(File.ReadAllLines(dialog.FileName));

			_FilesOpen.Add(newFile);

			if (newFile._TextSymbols == null)
			{
				MessageBox.Show("There are no TEXT symbols in this map file!");
				return;
			}

			DialogResult invertInputResult = MessageBox.Show("Reverse the order of all functions?", "Urgent", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

			/* Populate tree with text data
       *
       * (FILENAME)
       *  - (FILE)
       *    - (SYMBOL)
       *      - (ADDRESS)
       *      - (SIZE)
       *      - (PARAMETERS)
       */

			Update();
			MapTree.BeginUpdate();

			// (FILENAME)
			TreeNode parent = new TreeNode(dialog.FileName)
			{
				Tag = "File",
				ImageIndex = 0,
			};

			List<string> pathsDone = new List<string>();
			int currentPath = -1;

			foreach (MAPParsing.TextEntry node in newFile._TextSymbols)
			{
				if (!pathsDone.Contains(node._Path))
				{
					// (FILE)
					TreeNode fileParent = new TreeNode(node._Path)
					{
						Tag = "Path",
						ImageIndex = 1,
					};
					parent.Nodes.Add(fileParent);

					pathsDone.Add(node._Path);
					currentPath++;
				}

				// (SYMBOL)
				TreeNode symbol = new TreeNode(node._SymbolDemangled)
				{
					Tag = "Function",
					ImageIndex = 2,
				};

				// (ADDRESS) / (SIZE)
				symbol.Nodes.Add(new TreeNode($"Address - {node._Address}"));
				symbol.Nodes.Add(new TreeNode($"Size - {node._Size}"));

				List<string> types = SYMParsing.GetTypes(node._SymbolDemangled);
				if (types != null)
				{
					foreach (string parameter in types)
					{
						// (PARAMETERS)
						symbol.Nodes.Add(new TreeNode($"Parameter Type - {parameter}"));
					}
				}

				if (invertInputResult == DialogResult.Yes)
				{
					parent.Nodes[currentPath].Nodes.Insert(0, symbol);
				}
				else
				{
					parent.Nodes[currentPath].Nodes.Add(symbol);
				}
			}

			MapTree.Nodes.Add(parent);
			MapTree.EndUpdate();
			DebugLog.Text += "Done!\n";

			_SelectedFile = _FilesOpen.Count;
		}

		private void commonFSCleanerToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new DecompFillerForm().Show();
		}
	}
}
