using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using RemoteVideoPlayer.Helpers;

namespace RemoteVideoPlayer.Views
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App
	{
		public static readonly List<string> Args = new List<string>();

		private static readonly SemaphoreSlim _logSemaphore = new SemaphoreSlim(1, 1);

		private static readonly Mutex _mutex = new Mutex(true, Assembly.GetExecutingAssembly().GetName().Name);

		public App()
		{
			if (!_mutex.WaitOne(TimeSpan.Zero, true))
			{
				this.Shutdown();
				return;
			}

			this.Exit += AppExit;
			PlayerServiceManager.Current.Start();
			this.ShutdownMode = ShutdownMode.OnMainWindowClose;
		}

		private static void AppExit(object sender, ExitEventArgs e)
		{
			PlayerServiceManager.Current.CloseServer();
			_mutex.Close();
		}

		private void AppStartup(object sender, StartupEventArgs e)
		{
			if (e.Args.Length > 0)
			{
				foreach (var arg in e.Args)
				{
					Args.Add(arg.Trim('"'));
				}
			}
		}

		public static void WriteLog(object msg)
		{
			if (!ConfigHelper.DebugMode)
			{
				return;
			}

			_logSemaphore.Wait();

			try
			{
				var directory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				using (var sw = new StreamWriter(Path.Combine(directory ?? "", "log.txt"), true))
				{
					sw.WriteLine($"{DateTime.Now:HH:mm:ss}: {msg}");
					sw.Flush();
				}
			}
			finally
			{
				_logSemaphore.Release();
			}
		}
	}
}
