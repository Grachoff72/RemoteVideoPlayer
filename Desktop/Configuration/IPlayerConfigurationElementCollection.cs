using System.Collections.Generic;

namespace RemoteVideoPlayer.Configuration
{
	public interface IPlayerConfigurationElementCollection
	{
		void AddRange<T>(IEnumerable<T> elements) where T : MovieItemElement;

		void Clear();
	}
}