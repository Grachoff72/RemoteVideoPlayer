using System;
using System.Collections.Generic;

namespace RemoteVideoPlayer.Models
{
	[Serializable]
	public class Movie : MovieListItem
	{
		private TimeSpan _position;

		public TimeSpan Position
		{
			get => this._position;
			set { this._position = value; this.OnPropertyChanged(nameof(this.Position)); }
		}

		private List<Subtitle> _subtitles;

		public List<Subtitle> Subtitles
		{
			get => this._subtitles;
			set { this._subtitles = value; this.OnPropertyChanged(nameof(this.Subtitles)); }
		}

		public Movie(string path) : base(path)
		{
			this.Name = System.IO.Path.GetFileNameWithoutExtension(Uri.UnescapeDataString(path));

			this._subtitles = new List<Subtitle>();
		}
	}
}