using System;

namespace RemoteVideoPlayer.Win32
{
	[Flags]
	public enum ExitWindows : uint
	{
		// ONE of the following five:
		LogOff = 0x00,
		ShutDown = 0x01,
		Reboot = 0x02,
		PowerOff = 0x08,
		RestartApps = 0x40,
		// plus AT MOST ONE of the following two:
		Force = 0x04,
		ForceIfHung = 0x10,
	}
}