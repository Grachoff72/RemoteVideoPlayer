using System.ServiceModel;

namespace RemoteVideoPlayer.WCF
{
	// NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IRemoteVideoPlayerService" in both code and config file together.
	[ServiceContract(/*SessionMode = SessionMode.Required*/)]
	public interface IRemoteVideoPlayerService
	{
		[OperationContract(IsOneWay = true)]
		void Play();

		[OperationContract(IsOneWay = true)]
		void LevelUp();

		[OperationContract(IsOneWay = true)]
		void SelectMovie();

		[OperationContract(IsOneWay = true)]
		void Skip();

		[OperationContract(IsOneWay = true)]
		void SkipBack();

		[OperationContract(IsOneWay = true)]
		void CloseApp();

		[OperationContract(IsOneWay = true)]
		void FastForward();

		[OperationContract(IsOneWay = true)]
		void Rewind();

		[OperationContract(IsOneWay = true)]
		void ChangeScreenState();

		[OperationContract(IsOneWay = true)]
		void Mute();

		[OperationContract(IsOneWay = true)]
		void VolumeDown();

		[OperationContract(IsOneWay = true)]
		void VolumeUp();

		[OperationContract(IsOneWay = true)]
		void Pause();

		[OperationContract(IsOneWay = true)]
		void Stop();

		[OperationContract(IsOneWay = true)]
		void Next();

		[OperationContract(IsOneWay = true)]
		void Previous();

		[OperationContract(IsOneWay = true)]
		void SelectItem();

		[OperationContract(IsOneWay = true)]
		void Info();

		[OperationContract(IsOneWay = true)]
		void PageDown();

		[OperationContract(IsOneWay = true)]
		void PageUp();

		[OperationContract(IsOneWay = true)]
		void Suspend();
	}
}
