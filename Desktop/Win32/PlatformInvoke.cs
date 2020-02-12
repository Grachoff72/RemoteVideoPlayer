using System.Runtime.InteropServices;

namespace RemoteVideoPlayer.Win32
{
	public static class PlatformInvoke
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		internal static extern ExecutionState SetThreadExecutionState(ExecutionState esFlags);

		[DllImport("powrprof.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
		public static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

		[DllImport("user32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ExitWindowsEx(ExitWindows uFlags, uint dwReason);
	}
}