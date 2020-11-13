using System;
using System.ComponentModel;

namespace RemoteVideoPlayer.Models
{
	[Serializable]
	public class MovieListItem : INotifyPropertyChanged
	{
		private string _name;
		private string _path;

		public string Name
		{
			get => this._name;
			set { this._name = value; this.OnPropertyChanged(nameof(this.Name)); }
		}

		public string Path
		{
			get => this._path;
			set { this._path = value; this.OnPropertyChanged(nameof(this.Path)); }
		}

		protected MovieListItem() { }

		protected MovieListItem(string path)
		{
			this.Path = path;
		}

		protected void OnPropertyChanged(string name)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}