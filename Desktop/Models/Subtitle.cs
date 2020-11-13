using System;

namespace RemoteVideoPlayer.Models
{
	public class Subtitle
	{
		public TimeSpan Begin { get; set; }

		public TimeSpan End { get; set; }

		public string Text { get; set; }
	}
}