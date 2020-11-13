using System;
using System.Collections.Concurrent;
using System.Messaging;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using RemoteVideoPlayer.Helpers;
using RemoteVideoPlayer.Views;

namespace RemoteVideoPlayer
{
	public class PlayerServiceManager
	{
		private ServiceHost _host;

		private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

		private static readonly ConcurrentStack<IPlayer> _facilities = new ConcurrentStack<IPlayer>();

		private static PlayerServiceManager _current;

		private MessageQueue _inputQueue;

		public static PlayerServiceManager Current => _current ?? (_current = new PlayerServiceManager());

		public static void AddFacility(IPlayer facility)
		{
			_facilities.Push(facility);

			App.WriteLog($"facility {facility} was add");
		}

		public static void RemoveLastFacility()
		{
			if (_facilities.Count == 1)
			{
				return;
			}

			_facilities.TryPop(out var facility);

			App.WriteLog($"facility {facility} was removed");
		}

		public static IPlayer GetActiveFacility()
		{
			_facilities.TryPeek(out var facility);

			return facility;
		}

		public void Start()
		{
			this.CreateQueues();

			Task.Factory.StartNew(this.InitHost, this._tokenSource.Token);
		}

		private void InitHost()
		{
			try
			{
				this._host = new ServiceHost(typeof(VideoServer));

				this._host.Opened += this.HostOpened;
				this._host.Closed += this.HostClosed;
				this._host.Faulted += this.HostFaulted;
				this._host.UnknownMessageReceived += this.HostUnknownMessageReceived;

				this._host.Open();
			}
			catch (Exception ex)
			{
				App.WriteLog(ex);
			}
		}

		private void HostUnknownMessageReceived(object sender, UnknownMessageReceivedEventArgs e)
		{
			App.WriteLog(e.Message);
		}

		private void HostFaulted(object sender, EventArgs e)
		{
			App.WriteLog(sender);
			App.WriteLog(e);
		}

		public void CloseServer()
		{
			try
			{
				this._tokenSource.Cancel();
				this._tokenSource.Dispose();

				this.CloseHost();
				this.CloseQueues();
			}
			catch (ObjectDisposedException) { }
			catch (CommunicationException) { }
		}

		private void CloseHost()
		{
			_host?.Close();
		}

		private void HostOpened(object sender, EventArgs e)
		{
			App.WriteLog("host opened");
		}

		private void HostClosed(object sender, EventArgs e)
		{
			App.WriteLog("host closed");
		}

		private void CreateQueues()
		{
			try
			{
				var service = new ServiceController("MSMQ");

				if (service.Status != ServiceControllerStatus.Running)
				{
					service.Start();
					service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
				}
			}
			catch (Exception ex)
			{
				//MessageBox.Show(
				//	"Служба очереди сообщений отключена или недоступна",
				//	"Remote Video Player",
				//	MessageBoxButton.OK,
				//	MessageBoxImage.Warning
				//	);

				App.WriteLog(ex);
			}

			this._inputQueue = CreateQueue(ConfigHelper.InputQueueName);
			//this._outputQueue = CreateQueue(ConfigHelper.OutputQueueName);
		}

		private MessageQueue CreateQueue(string queueName)
		{
			MessageQueue queue = null;

			try
			{
				if (MessageQueue.Exists(queueName))
				{
					queue = new MessageQueue(queueName, QueueAccessMode.SendAndReceive);
					queue.Purge();
				}
				else
				{
					using (var tmp = MessageQueue.Create(queueName, true))
					{
						//queue.Label = "RemoteVideoPlayer Queue";
						tmp.DefaultPropertiesToSend.TimeToBeReceived = TimeSpan.FromSeconds(1);
						tmp.DefaultPropertiesToSend.UseDeadLetterQueue = false;
						tmp.DefaultPropertiesToSend.Recoverable = false;

						queue = tmp;
					}
				}

				App.WriteLog($"queue {queueName} created");
			}
			catch (Exception ex)
			{
				App.WriteLog(ex);
			}

			return queue;
		}

		private void CloseQueues()
		{
			this._inputQueue?.Close();
			App.WriteLog("input queue closed");

			//this._outputQueue?.Close();
			//App.WriteLog("output queue closed");
		}
	}
}