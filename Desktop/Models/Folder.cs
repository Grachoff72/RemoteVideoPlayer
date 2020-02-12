using System;
using System.IO;

namespace RemoteVideoPlayer.Models
{
	[Serializable]
	public class Folder : MovieListItem
	{
		//public List<MovieListItem> Items { get; set; }

		private bool _isParent;
		private bool _isRoot;

		public bool IsParent
		{
			get { return this._isParent; }
			set { this._isParent = value; this.OnPropertyChanged(nameof(this.IsParent)); }
		}

		public bool IsRoot
		{
			get { return this._isRoot; }
			set { this._isRoot = value; this.OnPropertyChanged(nameof(this.IsRoot));}
		}

		public Folder(string path, bool isParent = false) : base(path)
		{
			this.IsParent = isParent;
			this.Name = isParent ? ".." : new DirectoryInfo(path).Name;
		}
	}
}