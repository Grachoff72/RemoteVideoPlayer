namespace RemoteVideoPlayer.Views
{
	public interface IPlayerCommandProcessor : IPlayer
	{
		void SelectMovie();

		void Play();

		void Skip();

		void SkipBack();

		void FastForward();

		void Rewind();

		void ChangeScreenState();

		void Mute();

		void VolumeDown();

		void VolumeUp();

		void Pause();

		void Stop();

		void Next();

		void Previous();

		void ShowRoot();

		void Suspend();
	}
}