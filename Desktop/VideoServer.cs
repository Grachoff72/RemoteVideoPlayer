using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Threading;
using RemoteVideoPlayer.Views;
using RemoteVideoPlayer.WCF;

namespace RemoteVideoPlayer
{
	[ServiceBehavior(/*InstanceContextMode = InstanceContextMode.PerCall,
		TransactionIsolationLevel = IsolationLevel.ReadCommitted,
		ReleaseServiceInstanceOnTransactionComplete = false,
		TransactionAutoCompleteOnSessionClose = true*/)]
	public class VideoServer : IRemoteVideoPlayerService
	{
		#region IRemoteVideoPlayerService Implementation

		void IRemoteVideoPlayerService.Play()
		{
			InvokeAction(nameof(IPlayerCommandProcessor.Play));
		}

		public void LevelUp()
		{
			var window = PlayerServiceManager.GetActiveFacility();

			if (window is IListBoxWindow)
			{
				window.WindowDispatcher.Invoke(((IListBoxWindow)window).LevelUp);
			}

			if (window is IPlayerCommandProcessor)
			{
				InvokeAction(nameof(IPlayerCommandProcessor.SelectMovie));
			}
		}

		void IRemoteVideoPlayerService.Pause()
		{
			InvokeAction(nameof(IPlayerCommandProcessor.Pause));
		}

		void IRemoteVideoPlayerService.SelectMovie()
		{
			var window = PlayerServiceManager.GetActiveFacility();

			if (window is IListBoxWindow)
			{
				return;
			}

			InvokeAction(nameof(IPlayerCommandProcessor.SelectMovie));
		}

		void IRemoteVideoPlayerService.Skip()
		{
			InvokeAction(nameof(IPlayerCommandProcessor.Skip));
		}

		void IRemoteVideoPlayerService.SkipBack()
		{
			InvokeAction(nameof(IPlayerCommandProcessor.SkipBack));
		}

		void IRemoteVideoPlayerService.CloseApp()
		{
			try
			{
				var window = PlayerServiceManager.GetActiveFacility();

				window.WindowDispatcher.Invoke(((Window)window).Close);
			}
			catch (Exception ex)
			{
				App.WriteLog(ex);
			}
		}

		void IRemoteVideoPlayerService.FastForward()
		{
			InvokeAction(nameof(IPlayerCommandProcessor.FastForward));
		}

		void IRemoteVideoPlayerService.Rewind()
		{
			InvokeAction(nameof(IPlayerCommandProcessor.Rewind));
		}

		void IRemoteVideoPlayerService.ChangeScreenState()
		{
			InvokeAction(nameof(IPlayerCommandProcessor.ChangeScreenState));
		}

		void IRemoteVideoPlayerService.Mute()
		{
			InvokeAction(nameof(IPlayerCommandProcessor.Mute));
		}

		void IRemoteVideoPlayerService.VolumeDown()
		{
			InvokeAction(nameof(IPlayerCommandProcessor.VolumeDown));
		}

		void IRemoteVideoPlayerService.VolumeUp()
		{
			InvokeAction(nameof(IPlayerCommandProcessor.VolumeUp));
		}

		void IRemoteVideoPlayerService.Stop()
		{
			InvokeAction(nameof(IPlayerCommandProcessor.Stop));
		}

		void IRemoteVideoPlayerService.Next()
		{
			try
			{
				var window = PlayerServiceManager.GetActiveFacility();

				if (window is IPlayerCommandProcessor)
				{
					InvokeAction(nameof(IPlayerCommandProcessor.Next));
					return;
				}

				if (window is IListBoxWindow)
				{
					window.WindowDispatcher.Invoke(((IListBoxWindow)window).Next);
				}

				App.WriteLog($"{nameof(IRemoteVideoPlayerService.Next)}: done");
			}
			catch (Exception ex)
			{
				App.WriteLog(ex);
			}
		}

		void IRemoteVideoPlayerService.Previous()
		{
			try
			{
				var window = PlayerServiceManager.GetActiveFacility();

				if (window is IPlayerCommandProcessor)
				{
					InvokeAction(nameof(IPlayerCommandProcessor.Previous));
					return;
				}

				if (window is IListBoxWindow)
				{
					window.WindowDispatcher.Invoke(((IListBoxWindow)window).Previous);
				}

				App.WriteLog($"{nameof(IRemoteVideoPlayerService.Previous)}: done");
			}
			catch (Exception ex)
			{
				App.WriteLog(ex);
			}
		}

		public void SelectItem()
		{
			try
			{
				var window = PlayerServiceManager.GetActiveFacility();

				if (window is IListBoxWindow)
				{
					window.WindowDispatcher.Invoke(((IListBoxWindow)window).SelectItem);
				}

				App.WriteLog($"{nameof(IRemoteVideoPlayerService.SelectItem)}: done");
			}
			catch (Exception ex)
			{
				App.WriteLog(ex);
			}
		}

		public void PageDown()
		{
			try
			{
				var window = PlayerServiceManager.GetActiveFacility();

				if (window is IListBoxWindow)
				{
					window.WindowDispatcher.Invoke(((IListBoxWindow)window).PageDown);
				}

				App.WriteLog($"{nameof(IRemoteVideoPlayerService.PageDown)}: done");
			}
			catch (Exception ex)
			{
				App.WriteLog(ex);
			}
		}

		public void PageUp()
		{
			try
			{
				var window = PlayerServiceManager.GetActiveFacility();

				if (window is IListBoxWindow)
				{
					window.WindowDispatcher.Invoke(((IListBoxWindow)window).PageUp);
				}

				App.WriteLog($"{nameof(IRemoteVideoPlayerService.PageUp)}: done");
			}
			catch (Exception ex)
			{
				App.WriteLog(ex);
			}
		}

		public void Info()
		{
			var window = PlayerServiceManager.GetActiveFacility();

			if (window is IPlayerCommandProcessor)
			{
				window.WindowDispatcher.Invoke(((IPlayerCommandProcessor)window).ShowRoot);
			}
		}

		public void Suspend()
		{
			var window = PlayerServiceManager.GetActiveFacility();

			if (window is IPlayerCommandProcessor)
			{
				window.WindowDispatcher.Invoke(((IPlayerCommandProcessor)window).Suspend);
			}
		}

		#endregion

/*
		private static void ProcessTransaction<T>(string methodName)
		{
			var client = new PlayerCommandResultClient();

			using (var scope = new TransactionScope(TransactionScopeOption.Required))
			{
				var result = InvokeFunction<T>(methodName);

				client.SendResult(result);

				scope.Complete();
			}
		}
*/

		private static void InvokeAction(string methodName, DispatcherPriority priority = DispatcherPriority.Normal)
		{
			try
			{
				App.WriteLog($"Request received: {methodName}");

				var facility = PlayerServiceManager.GetActiveFacility();

				if (!(facility is IPlayerCommandProcessor))
				{
					return;
				}

				var method = typeof(IPlayerCommandProcessor).GetMethod(methodName);

				if (method == null)
				{
					return;
				}

				facility.WindowDispatcher?.BeginInvoke(new Action(((IPlayerCommandProcessor)facility).ShowRoot));

				var action = (Action)Delegate.CreateDelegate(typeof(Action), facility, method);

				facility.WindowDispatcher?.Invoke(action, priority);
			}
			catch (Exception ex)
			{
				App.WriteLog(ex);
			}
		}
	}
}