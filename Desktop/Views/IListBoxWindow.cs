namespace RemoteVideoPlayer.Views
{
	public interface IListBoxWindow : IPlayer
	{
		void Next();

		void Previous();

		void SelectItem();

		void LevelUp();

		void PageUp();

		void PageDown();
	}
}