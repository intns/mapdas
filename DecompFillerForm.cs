using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mapdas
{
	public partial class DecompFillerForm : Form
	{
		public DecompFillerForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			CommonOpenFileDialog dialog = new CommonOpenFileDialog
			{
				IsFolderPicker = true,
				Title = "Select mapdas output folder to clean..."
			};

			if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
			{
				return;
			}

			richTextBox1.Text = dialog.FileName;
		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{

		}

		private void button2_Click(object sender, EventArgs e)
		{
			if (richTextBox1.Text == string.Empty)
			{
				MessageBox.Show("Enter the mapdas output folder!");
				return;
			}

			List<string> namespaces = new List<string>();
			namespaces = richTextBox2.Text.Split(new char[] { ',', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
			MessageBox.Show("Starting, wait until the \"Done\" message pops up to do anything else!");
			CommonDecompFiller.Filler.Run(richTextBox1.Text, namespaces);
			MessageBox.Show("Done");
		}
	}
}
