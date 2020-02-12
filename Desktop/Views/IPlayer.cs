using System.Windows.Threading;

namespace RemoteVideoPlayer.Views
{
	public interface IPlayer
	{
		Dispatcher WindowDispatcher { get; }
	}
}