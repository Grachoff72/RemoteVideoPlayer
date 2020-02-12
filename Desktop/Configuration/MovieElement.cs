using System;
using System.Configuration;

namespace RemoteVideoPlayer.Configuration
{
	public class MovieElement : MovieItemElement
	{
		private const string POSITION = "position";

		[ConfigurationProperty(POSITION, IsRequired = false, IsDefaultCollection = false)]
		public TimeSpan Position
		{
			get
			{
				TimeSpan ts;

				TimeSpan.TryParse(base[POSITION]?.ToString() ?? String.Empty, out ts);

				return ts;
			}

			set { base[POSITION] = value.ToString(@"h\:mm\:ss"); }
		}
	}
}