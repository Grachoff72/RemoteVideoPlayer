using System.Configuration;

namespace RemoteVideoPlayer.Configuration
{
	public class MovieItemElement : ConfigurationElement
	{
		private const string PATH = "path";

		[ConfigurationProperty(PATH, IsRequired = true, IsKey = true, IsDefaultCollection = false)]
		public string Path
		{
			get { return (string)base[PATH]; }
			set { base[PATH] = value; }
		}
	}
}