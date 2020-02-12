using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebPanel
{
	public class MvcApplication : HttpApplication
	{
		public static bool IsNowPlaying { get; set; }

		//public static readonly PlayerCommandResult Player = new PlayerCommandResult();

		protected void Application_Start()
		{
			//AreaRegistration.RegisterAllAreas();
			//FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
			//RouteConfig.RegisterRoutes(RouteTable.Routes);
			//BundleConfig.RegisterBundles(BundleTable.Bundles);
		}

		public static void WriteLog(object msg)
		{
			//var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			using (var sw = new StreamWriter(Path.Combine(@"c:\temp\RemoteVideoPlayer", "log.txt"), true))
			{
				sw.WriteLine($"{DateTime.Now:HH:mm:ss}: {msg}");
				sw.Flush();
			}
		}

		public static bool RunPlayer()
		{
			try
			{
				var processes = Process.GetProcessesByName("RemoteVideoPlayer");

				if (!processes.Any())
				{
					Process.Start(@"f:\Projects\RemoteVideoPlayer\Desktop\bin\Debug\RemoteVideoPlayer.exe");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);

				return false;
			}

			return true;
		}
	}
}
