using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using RemoteVideoPlayer.Helpers;
using RemoteVideoPlayer.Models;
using Button = System.Windows.Controls.Button;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using ListBox = System.Windows.Controls.ListBox;

namespace RemoteVideoPlayer.Views
{
	/// <summary>
	/// Interaction logic for MovieLisWindow.xaml
	/// </summary>
	public partial class MovieListWindow
	{
		public IOHelper FileHelper { get; }

		public Movie Movie { get; private set; }

		protected override ListBox ListBox => this.MovieListBox;

		public MovieListWindow(IOHelper ioHelper)
		{
			this.FileHelper = ioHelper;

			InitializeComponent();

			this.MovieListBox.DataContext = this.FileHelper;
			if (this.MovieListBox.Items.Count > 0)
			{
				this.MovieListBox.SelectedIndex = 0;
				this.FocusSelectedItem();
			}

			this.PreviewKeyUp += this.MovieListWindow_PreviewKeyUp;
		}

		private void MovieListWindow_PreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				this.InternalSelectItem();
			}
			else if (e.Key == Key.Back)
			{
				this.LevelUp();
			}
		}

		private void CloseButton_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void MovieListItemClick(object sender, RoutedEventArgs e)
		{
			var item = ((Button)sender).DataContext;

			this.SelectMovieListItem(item);
		}

		private void SelectMovieListItem(object item)
		{
			if (item is Folder)
			{
				var folder = (Folder)item;

				if (folder.IsParent)
				{
					this.LevelUp();
				}
				else
				{
					this.FileHelper.GetMovieList(((Folder)item).Path);

					if (this.MovieListBox.Items.Count > 0)
					{
						this.MovieListBox.SelectedIndex = 0;

						this.FocusSelectedFolder();
					}

					this.MovieListBox.Items.Refresh();
				}
			}
			else if (item is Movie)
			{
				this.Movie = (Movie)item;

				var savedMovie = this.FileHelper.GetSavedMovie(this.Movie.Path);

				if (savedMovie != null && savedMovie.Position > TimeSpan.Zero)
				{
					var pos = SelectMoviePositionWindow.DefineMoviePosition(savedMovie);

					if (pos.Span == null)
					{
						return;
					}

					this.Movie.Position = pos.Span.Value;
				}
				
				this.DialogResult = true;
				this.Close();
			}
		}

		private void FocusSelectedFolder()
		{
			this.MovieListBox.ScrollIntoView(this.MovieListBox.SelectedItem);

			var listBoxItem = (ListBoxItem)this.MovieListBox.ItemContainerGenerator.ContainerFromItem(this.MovieListBox.SelectedItem);
			listBoxItem?.Focus();
		}

		#region Overrides of ListBoxWindow

		public override Dispatcher WindowDispatcher => this.Dispatcher;

		public override void LevelUp()
		{
			var currentFolder = IOHelper.CurrentFolder;

			this.FileHelper.LevelUp();

			this.MovieListBox.Items.Refresh();

			var i = this.FileHelper.MovieList.FirstOrDefault(x => x.Path == currentFolder);

			if (i != null)
			{
				this.MovieListBox.SelectedItem = i;
				this.FocusSelectedFolder();
			}
		}

		protected override void InternalSelectItem()
		{
			if (this.MovieListBox.SelectedIndex < 0)
			{
				return;
			}

			this.SelectMovieListItem(this.MovieListBox.SelectedItem);
		}

		#endregion
	}
}
