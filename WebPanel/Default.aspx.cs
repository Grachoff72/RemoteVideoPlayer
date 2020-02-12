using System.Web.UI;

namespace WebPanel
{
	public class Default : Page
	{
		protected void Page_Load()
		{
			this.Response.Redirect("Player.aspx");
		}
	}
}