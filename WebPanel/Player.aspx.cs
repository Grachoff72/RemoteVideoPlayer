using System;
using System.Transactions;
using System.Web.UI;
using WebPanel.RemoteVideoPlayerReference;

namespace WebPanel
{
	public class Player : Page
	{
		protected void PlayButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.Play));
		}

		protected void PauseButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.Pause));
		}

		protected void PowerButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.Suspend));
		}

		protected void CloseButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.CloseApp));
		}

		protected void VolumeButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.Mute));
		}

		protected void ScreenButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.ChangeScreenState));
		}

		protected void LevelUpButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.LevelUp));
		}

		protected void VolumeUpButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.VolumeUp));
		}

		protected void VolumeDownButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.VolumeDown));
		}

		protected void NextButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.Next));
		}

		protected void OkButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.SelectItem));
		}

		protected void PrevButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.Previous));
		}

		protected void RewindButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.Rewind));
		}

		protected void ForwardButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.FastForward));
		}

		protected void UpButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.Previous));
		}

		protected void DownButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.Next));
		}

		protected void InfoButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.Info));
		}

		protected void LeftButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.SkipBack));
		}

		protected void RightButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.Skip));
		}

		protected void PageUpButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.PageUp));
		}

		protected void PageDownButton_Click(object sender, ImageClickEventArgs e)
		{
			SendAction(nameof(IRemoteVideoPlayerService.PageDown));
		}

		private static void SendAction(string actionName)
		{
			var client = new RemoteVideoPlayerServiceClient(/*"NetNamedPipeBinding_IRemoteVideoPlayerService"*/);

			try
			{
				//client.Open();
				using (var scope = new TransactionScope(TransactionScopeOption.Required))
				{
					var action = (Action)Delegate.CreateDelegate(typeof(Action), client, actionName);
					action.Invoke();

					scope.Complete();
				}

				MvcApplication.WriteLog($"action {actionName} done");
			}
			catch (Exception ex)
			{
				MvcApplication.WriteLog(ex);
			}
			finally
			{
				try
				{
					client.Close();
				}
				catch (Exception)
				{
					client.Abort();
				}
			}
		}
	}
}