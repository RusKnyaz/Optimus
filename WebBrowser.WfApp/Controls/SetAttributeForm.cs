using System.Windows.Forms;

namespace WebBrowser.WfApp.Controls
{
	public partial class SetAttributeForm : Form
	{
		public SetAttributeForm()
		{
			InitializeComponent();
		}

		public Dom.Elements.HtmlElement Element { get; set; }
		
		private void button2_Click(object sender, System.EventArgs e)
		{
			if (Element == null) return;
			var attributeName = textBoxName.Text;
			var attributeValue = textBoxName.Text;
			Element.SetAttribute(attributeName, attributeValue);
			Close();
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			Close();
		}
	}
}
