using System.Windows.Forms;

namespace mapdas
{
	public partial class DisasmView : Form
	{
		public DisasmView(string funcName = "", string hex = "", string ppc = "")
		{
			InitializeComponent();

			funcIdentityView.Text = funcName;
			hexView.Text = hex;
			ppcView.Text = ppc;
			Icon = ActiveForm.Icon;
			MinimizeBox = false;
			MaximizeBox = false;
		}
	}
}
