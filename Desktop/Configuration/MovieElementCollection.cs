using System.Configuration;

namespace RemoteVideoPlayer.Configuration
{
	[ConfigurationCollection(
	typeof(MovieElementCollection),
	AddItemName = "movie",
	CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class MovieElementCollection : MovieItemElementCollection
	{
		#region Overrides of MovieItemElementCollection

		/// <summary>When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.</summary>
		/// <returns>A newly created <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new MovieElement();
		}

		#endregion
	}
}