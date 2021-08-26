using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using RemoteVideoPlayer.Models;

namespace RemoteVideoPlayer.Helpers
{
	public class IOHelper
	{
		public List<MovieListItem> MovieList { get; set; }

		private readonly List<Movie> _savedMovies;

		private readonly List<string> _currentMovies = new List<string>();

		public static string CurrentFolder
		{
			get
			{
				var currentFolder = ConfigHelper.CurrentFolder;

				if (!Directory.Exists(currentFolder))
				{
					currentFolder = String.Empty;
					ConfigHelper.CurrentFolder = currentFolder;
				}

				return currentFolder;
			}
			private set => ConfigHelper.CurrentFolder = value;
		}

		public string CurrentFile { get; private set; }

		private int CurrentMovieIndex { get; set; }

		private List<Folder> RootFolders { get; }

		public IOHelper()
		{
			this.RootFolders = ConfigHelper.RootFolders.ToList();
			var removeCount = this.RootFolders.RemoveAll(x => !Directory.Exists(x.Path));

			if (removeCount > 0)
			{
				ConfigHelper.RootFolders = this.RootFolders;
			}

			this._savedMovies = ConfigHelper.Movies.ToList();

			this.MovieList = new List<MovieListItem>();
			this.GetMovieList(CurrentFolder);

			this.UpdateSavedMovies();
		}

		internal void UpdateSavedMovies()
		{
			var removeCount = this._savedMovies.RemoveAll(x => !File.Exists(x.Path));

			if (removeCount > 0)
			{
				ConfigHelper.Movies = this._savedMovies;
			}
		}

		public string GetNextFile(bool forward)
		{
			if (forward)
			{
				this.CurrentMovieIndex++;

				if (this.CurrentMovieIndex >= this._currentMovies.Count)
				{
					return String.Empty;
				}
			}
			else
			{
				this.CurrentMovieIndex--;

				if (this.CurrentMovieIndex < 0)
				{
					return String.Empty;
				}
			}

			return this._currentMovies[this.CurrentMovieIndex];
		}

		internal static void SearchSubtitles(Movie movie)
		{
			try
			{
				var dir = Path.GetDirectoryName(movie.Path);
                var files = Directory.GetFiles(dir ?? "", $"{movie.Name}*.srt");

				var subtitlesPath = files.FirstOrDefault();

				if (subtitlesPath == null)
				{
					return;
				}

				using (var sr = File.OpenText(subtitlesPath))
				{
					Subtitle subtitle = null;

					while (!sr.EndOfStream)
					{
						var line = sr.ReadLine() ?? "";

						if (Regex.IsMatch(line, @"^\d+$"))
						{
							subtitle = new Subtitle();
							continue;
						}

						if (Regex.IsMatch(line, @"^\d{2}\:\d{2}\:\d{2}\,\d{3}\s-->\s\d{2}\:\d{2}\:\d{2}\,\d{3}$") && subtitle != null)
						{
							var spans = line.Split(new[] { "-->" }, StringSplitOptions.RemoveEmptyEntries);

							if (spans.Length != 2)
							{
								subtitle = null;
								continue;
							}

							TimeSpan.TryParse(spans[0].Trim(), out var begin);
							TimeSpan.TryParse(spans[1].Trim(), out var end);

							if (end <= begin)
							{
								subtitle = null;
								continue;
							}

							subtitle.Begin = begin;
							subtitle.End = end;

							continue;
						}

						if (String.IsNullOrEmpty(line))
						{
							if (subtitle != null)
							{
								movie.Subtitles.Add(subtitle);
							}

							subtitle = null;
							continue;
						}

						if (subtitle != null)
						{
							subtitle.Text = $"{subtitle.Text}{(String.IsNullOrEmpty(subtitle.Text) ? "" : "\r\n")}{line.RemoveHtmlTags()}" ;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		private void UpdateMovieList(string file)
		{
			var folder = new FileInfo(file).Directory;

			this._currentMovies.Clear();

			var movieList = GetMovieFileList(folder?.FullName);

			this._currentMovies.AddRange(movieList.Select(x => x.Path));

			this.CurrentMovieIndex = movieList.FindIndex(x => x.Path == file);
		}

		public void GetMovieList(string currentFolder)
		{
			this.MovieList.Clear();

			if (String.IsNullOrEmpty(currentFolder))
			{
				this.MovieList.AddRange(this.RootFolders);

				CurrentFolder = String.Empty;

				return;
			}

			CurrentFolder = currentFolder;

			var dirs = Directory.EnumerateDirectories(CurrentFolder).Select(x => new Folder(x));

			this.MovieList.AddRange(dirs);

			var fileList = GetMovieFileList(CurrentFolder);

			this.MovieList.AddRange(fileList);

			this.MovieList.Insert(0, new Folder(currentFolder, true));
		}

		public Movie GetSavedMovie(string path)
		{
			var movie = new Movie(path);

			var savedMovie = this._savedMovies.FirstOrDefault(x => x.Path == movie.Path);
			return savedMovie;
		}

		public void SaveMovie(Movie movie)
		{
			var currentMovie = this._savedMovies.FirstOrDefault(x => x.Path == movie.Path);

			if (currentMovie == null)
			{
				this._savedMovies.Add(movie);
			}
			else
			{
				if ((int)movie.Position.TotalSeconds == 0)
				{
					this._savedMovies.Remove(movie);
				}
				else
				{
					currentMovie.Position = movie.Position;
				}
			}

			ConfigHelper.Movies = this._savedMovies;
		}

		private static List<Movie> GetMovieFileList(string currentFolder)
		{
			if (!Directory.Exists(currentFolder))
			{
				return new List<Movie>();
			}

			var subList = new List<Movie>();

			var extensions = GetVideoExtensions();

			foreach (var ext in extensions)
			{
				var e = ext.Trim();

				if (!e.StartsWith("."))
				{
					e = e.Insert(0, ".");
				}

				var files = Directory.EnumerateFiles(currentFolder, $"*{e}");

				subList.AddRange(files.Select(x => new Movie(x)));
			}

			subList.Sort((x, y) => String.Compare(x.Path, y.Path, StringComparison.OrdinalIgnoreCase));

			return subList;
		}

		private static IEnumerable<String> GetVideoExtensions()
		{
			var extensions = ConfigHelper.KnownExtensions;

			return extensions.Split(new[]
			{
				',', '\u0020'
			}, StringSplitOptions.RemoveEmptyEntries);
		}

		public void SetCurrentFile(string file)
		{
			this.CurrentFile = file;

			this.UpdateMovieList(file);
		}

		public void LevelUp()
		{
			if(String.IsNullOrEmpty(CurrentFolder))
			{
				return;
			}

			var parent = new DirectoryInfo(CurrentFolder).Parent;

			if (parent == null)
			{
				return;
			}

			if (this.RootFolders.Any(x => x.Path == CurrentFolder))
			{
				this.GetMovieList(String.Empty);
				return;
			}

			this.GetMovieList(parent.FullName);
		}
	}
}