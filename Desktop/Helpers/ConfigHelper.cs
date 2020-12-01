using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using RemoteVideoPlayer.Configuration;
using RemoteVideoPlayer.Models;

namespace RemoteVideoPlayer.Helpers
{
	public static class ConfigHelper
	{
		private const string KNOWN_EXTENSIONS_NAME = "knownExtensions";

		private const string CURRENT_FOLDER_NAME = "currentFolder";

		private const string INPUT_QUEUE_NAME = "queueName";

		private const string DEBUG_MODE = "debugMode";

		private const string LAST_VOLUME = "lastVolume";

		public static System.Configuration.Configuration Config =>
			ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

		public static string KnownExtensions
		{
			get
			{
				var extensions = Config.AppSettings.Settings[KNOWN_EXTENSIONS_NAME]?.Value;

				if (String.IsNullOrEmpty(extensions))
				{
					extensions =
						".3g2, .3gp, .3gp2, .3gpp, .asf, .asr, .asx, .avi, .axv, .dif, .divx, .dv, .f4v, .flv, .ivf, .lsf, .lsx, .m1v, .m2t, .m2ts, .m2v, .m4v, .mk3d, .mkv, .mod, .mov, .movie, .mp2, .mp2v, .mp4, .mp4v, .mpa, .mpe, .mpeg, .mpg, .mpv2, .mqv, .mts, .nsc, .ogv, .qt, .ts, .tts, .vbk, .webm, .wm, .wmp, .wmv, .wmx, .wvx";
					SetAppSetting(KNOWN_EXTENSIONS_NAME, extensions);
				}

				return extensions;
			}
		}

		public static string CurrentFolder
		{
			get => Config.AppSettings.Settings[CURRENT_FOLDER_NAME]?.Value ?? String.Empty;
			set => SetAppSetting(CURRENT_FOLDER_NAME, value);
		}

		public static bool DebugMode
		{
			get
			{
				Boolean.TryParse(Config.AppSettings.Settings[DEBUG_MODE]?.Value ?? String.Empty, out var b);
				return b;
			}

			set => SetAppSetting(DEBUG_MODE, value.ToString().ToLower());
		}

		public static double LastVolume
		{
			get
			{
				Double.TryParse(Config.AppSettings.Settings[LAST_VOLUME]?.Value ?? "1.0", NumberStyles.Float, CultureInfo.InvariantCulture, out var d);
				return d;
			}
			set => SetAppSetting(LAST_VOLUME, value.ToString("F2", CultureInfo.InvariantCulture));
		}

		private static void SetAppSetting(string settingName, string value)
		{
			var config = Config;

			config.AppSettings.Settings.Remove(settingName);
			config.AppSettings.Settings.Add(settingName, value);

			config.Save(ConfigurationSaveMode.Modified);

			ConfigurationManager.RefreshSection("appSettings");
		}

		public static IEnumerable<Folder> RootFolders
		{
			get
			{
				var folderElements = RemoteVideoPlayerSection.Current.FolderElements;
				return folderElements.Cast<MovieItemElement>().Select(x => new Folder(x.Path));
			}
			set
			{
				var folders =
					value.Select(x => new MovieItemElement { Path = x.Path });

				SaveCollection(folders);
			}
		}

		public static IEnumerable<Movie> Movies
		{
			get
			{
				var movieElements = RemoteVideoPlayerSection.Current.MovieElements;

				return movieElements.Cast<MovieElement>().Select(x => new Movie(x.Path) { Position = x.Position });
			}
			set
			{
				var currentMovies =
					value.Select(x => new MovieElement { Path = x.Path, Position = x.Position });

				SaveCollection(currentMovies);
			}
		}

		private static void SaveCollection<T>(IEnumerable<T> collectionItems) where T : MovieItemElement
		{
			var config = Config;

			var movieSection = config.GetSection(RemoteVideoPlayerSection.SECTION_NAME) as RemoteVideoPlayerSection;

			if (movieSection != null)
			{
				var collection = movieSection.GetElementCollection<T>();
				collection.Clear();
				collection.AddRange(collectionItems);
			}

			config.Save(ConfigurationSaveMode.Modified);

			ConfigurationManager.RefreshSection(RemoteVideoPlayerSection.SECTION_NAME);
		}

		public static string InputQueueName
		{
			get
			{
				var queueName = Config.AppSettings.Settings[INPUT_QUEUE_NAME]?.Value;
				if (String.IsNullOrEmpty(queueName))
				{
					queueName = @".\private$\RemoteVideoPlayerQueue";
					SetAppSetting(INPUT_QUEUE_NAME, queueName);
				}

				return queueName;
			}
		}
	}
}