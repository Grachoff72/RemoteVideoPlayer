using System;
using System.Web.UI.WebControls;

namespace WebPanel
{
	public class InfoTextBox : TextBox
	{
		public event Action<object, EventArgs> ContentChanged;

		public void OnContentChanged(object obj, EventArgs e)
		{
			this.Text = obj.ToString();
			this.ContentChanged?.Invoke(obj, e);
		}
	}
}