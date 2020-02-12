using System;

namespace RemoteVideoPlayer.Models
{
	[Serializable]
	public class Movie : MovieListItem
	{
		private TimeSpan _position;

		public TimeSpan Position
		{
			get { return this._position; }
			set { this._position = value; this.OnPropertyChanged(nameof(this.Position)); }
		}

		public Movie(string path) : base(path)
		{
			this.Name = System.IO.Path.GetFileNameWithoutExtension(Uri.UnescapeDataString(path));
		}
	}
}