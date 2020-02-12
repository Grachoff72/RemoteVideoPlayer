using System;
using System.Configuration;
using RemoteVideoPlayer.Helpers;

namespace RemoteVideoPlayer.Configuration
{
	public class RemoteVideoPlayerSection : ConfigurationSection
	{
		public const string SECTION_NAME = "remotevideoplayer.settings";

		private const string FOLDER_COLLECTION_NAME = "movieFolders";

		private const string MOVIE_COLLECTION_NAME = "currentMovies";

		public static RemoteVideoPlayerSection Current
		{
			get
			{
				var s = ConfigurationManager.GetSection(SECTION_NAME);

				var section = s as RemoteVideoPlayerSection;

				if (section == null)
				{
					section = new RemoteVideoPlayerSection();

					var config = ConfigHelper.Config;
					config.Sections.Add(SECTION_NAME, section);
					config.Save(ConfigurationSaveMode.Modified);
				}

				return section;
			}
		}

		#region Overrides of ConfigurationElement

		/// <summary>Gets a value indicating whether the <see cref="T:System.Configuration.ConfigurationElement" /> object is read-only.</summary>
		/// <returns>true if the <see cref="T:System.Configuration.ConfigurationElement" /> object is read-only; otherwise, false.</returns>
		public override Boolean IsReadOnly()
		{
			return false;
		}

		#endregion

		public IPlayerConfigurationElementCollection GetElementCollection<T>()
		{
			if (typeof(T) == typeof(MovieElement))
			{
				return this.MovieElements;
			}

			return this.FolderElements;
		}

		[ConfigurationProperty(MOVIE_COLLECTION_NAME, IsDefaultCollection = false)]
		public MovieElementCollection MovieElements => (MovieElementCollection)base[MOVIE_COLLECTION_NAME] ?? new MovieElementCollection();

		[ConfigurationProperty(FOLDER_COLLECTION_NAME, IsDefaultCollection = false)]
		public MovieItemElementCollection FolderElements => (MovieItemElementCollection)base[FOLDER_COLLECTION_NAME] ?? new MovieItemElementCollection();

		//[ConfigurationProperty(KNOWN_EXTENSIONS_NAME)]
		//public string KnownExtensions
		//{
		//	get { return (string)base[KNOWN_EXTENSIONS_NAME]; }
		//	set { this[KNOWN_EXTENSIONS_NAME] = value; }
		//}

		//[ConfigurationProperty(CURRENT_FOLDER_NAME)]
		//public string CurrentFolder
		//{
		//	get { return (string)base[CURRENT_FOLDER_NAME]; }
		//	set { this[CURRENT_FOLDER_NAME] = value; }
		//}

		//[ConfigurationProperty(QUEUE_NAME_NAME)]
		//public string QueueName
		//{
		//	get { return (string)base[QUEUE_NAME_NAME]; }
		//	set { this[QUEUE_NAME_NAME] = value; }
		//}
	}
}