using System;
using System.Windows.Forms;

namespace mapdas
{
	public partial class Form3 : Form
	{
		public Form3(string typeString)
		{
			InitializeComponent();

			typeBox.Text = typeString;
		}

		private void Form3_Load(object sender, EventArgs e)
		{

		}
	}
}
