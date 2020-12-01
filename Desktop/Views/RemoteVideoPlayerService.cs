using System;
using System.IO;
using System.Windows;
using RemoteVideoPlayer.Helpers;
using RemoteVideoPlayer.Models;
using RemoteVideoPlayer.Win32;

namespace RemoteVideoPlayer.Views
{
	public partial class MainWindow : IPlayerCommandProcessor
	{
		private readonly IOHelper _fileHelper;

		private bool _isNowPlaying;
		private bool _isMuted;

		public const double MIN_VOLUME = 0.2;
		private const double MAX_VOLUME = 1.0;

		public static readonly DependencyProperty LastVolumeProperty = DependencyProperty.Register(
			nameof(LastVolume), typeof(double), typeof(MainWindow), new PropertyMetadata(default(double)));

		public double LastVolume { get => (double)GetValue(LastVolumeProperty); set => SetValue(LastVolumeProperty, value); }

		private double MovieRatio => (double)this.Player.NaturalVideoWidth / this.Player.NaturalVideoHeight;

		public static readonly DependencyProperty CurrentMovieProperty = DependencyProperty.Register(
			nameof(CurrentMovie), typeof(Movie), typeof(MainWindow), new PropertyMetadata(default(Movie)));

		public Movie CurrentMovie { get => (Movie)GetValue(CurrentMovieProperty); set => SetValue(CurrentMovieProperty, value); }

		private bool IsMuted
		{
			get => this._isMuted;
			set
			{
				if (value)
				{
					if (!_isMuted)
					{
						this.LastVolume = this.Player.Volume;
						this.Player.Volume = MIN_VOLUME;
					}
				}
				else
				{
					this.Player.Volume = this.LastVolume;
				}

				this._isMuted = value;
			}
		}

		private void IncreasePosition()
		{
			var pos = this.Player.MediaPosition + TimeSpan.FromSeconds(20).Ticks;

			if (pos > this.Player.MediaDuration)
			{
				this.Player.MediaPosition = this.Player.MediaDuration;
				this.Stop();
				return;
			}

			this.Player.MediaPosition = pos;
		}

		private void DecreasePosition()
		{
			var pos = this.Player.MediaPosition - TimeSpan.FromSeconds(20).Ticks;

			if (pos < 0)
			{
				this.Player.MediaPosition = 0;
				this.Stop();
				return;
			}

			this.Player.MediaPosition = pos;
		}

		private void InternalOpen(string file)
		{
			this._fileHelper.SetCurrentFile(file);

			this.Player.Source = new Uri(file);

			this.Title = Path.GetFileNameWithoutExtension(file);

			this.Player.Volume = this.LastVolume;
		}

		private void InternalPlay(long position = 0)
		{
			this.PlayButton.Content = SetButtonContent("PauseImage");

			this.Stopped = false;

			this._isNowPlaying = true;

			this._fastPlayTimer.Stop();

			this.Player.Volume = this.LastVolume;

			this.Player.Play();

			if (position > 0)
			{
				this.Player.MediaPosition = position;
			}

			this.SetControlsEnabled(true);
			this.ShowRoot();
			PlatformInvoke.SetThreadExecutionState(ExecutionState.ES_DISPLAY_REQUIRED | ExecutionState.ES_CONTINUOUS);
		}

		private void PlayMovie(Movie movie)
		{
			IOHelper.SearchSubtitles(movie);

			this.CurrentMovie = movie;

			this.InternalOpen(movie.Path);

			this._isNowPlaying = true;

			this.InternalPlay(movie.Position.Ticks);
		}

		private void PlayNextFile(bool forward)
		{
			this.SaveMoviePosition();

			var file = this._fileHelper.GetNextFile(forward);

			if (String.IsNullOrEmpty(file))
			{
				this.Stop();
			}
			else
			{
				this.PlayMovie(new Movie(file));
			}
		}

		private void PlayPause()
		{
			if (String.IsNullOrEmpty(this._fileHelper.CurrentFile))
			{
				this.SelectMovie();
				return;
			}

			if (this._isNowPlaying)
			{
				this.Pause();
			}
			else
			{
				this.Play();
			}
		}

		private void UpdatePlayerParameters()
		{
			this.InfoBlock.Text = new Movie(this.Player.Source.AbsolutePath).Name;

			this.ProgressSlider.Maximum = new TimeSpan(this.Player.MediaDuration).TotalSeconds;

			this.Resize();
		}

		private void Wind(bool forward)
		{
			if (this.Stopped)
			{
				return;
			}

			this._isNowPlaying = false;
			this.Pause();
			this.SetControlsEnabled(false);
			this.IsMuted = true;
			this._fastPlayTimer.Tag = forward;
			this._fastPlayTimer.Start();
		}

		private void SaveMoviePosition()
		{
			if (!this.Player.HasVideo)
			{
				return;
			}

			var pos = this.Player.MediaPosition;

			if (pos == this.Player.MediaDuration)
			{
				pos = 0;
			}

			this.CurrentMovie.Position = new TimeSpan(pos);

			this._fileHelper.SaveMovie(this.CurrentMovie);
		}

		private void FullScreen()
		{
			if (this.WindowState == WindowState.Maximized && this.WindowStyle == WindowStyle.SingleBorderWindow)
			{
				this.WindowState = WindowState.Normal;
			}

			this.ScreenButton.Content = SetButtonContent("RestoreScreenImage");

			this.WindowStyle = WindowStyle.None;
			this.WindowState = WindowState.Maximized;
		}

		private void RestoreScreen()
		{
			this.ScreenButton.Content = SetButtonContent("ExpandImage");

			this.WindowStyle = WindowStyle.SingleBorderWindow;
			this.WindowState = WindowState.Normal;
		}

		private void Resize()
		{
			if (this.Stopped)
			{
				return;
			}

			var pnlWidth = this.DockPanel.ActualWidth;
			var pnlHeight = this.DockPanel.ActualHeight;

			var w = pnlWidth;
			var h = w / this.MovieRatio;

			if (h > pnlHeight)
			{
				h = pnlHeight;

				w = h * this.MovieRatio;
			}

			this.Player.Width = w;
			this.Player.Height = h;

			this.ProgressSlider.Width = this.DockPanel.ActualWidth - 220;
			this.InvalidateVisual();
		}

		#region IPlayerCommandProcessor Implementation

		private bool Stopped { get; set; } = true;

		public void SelectMovie()
		{
			this.Stop();

			this._fileHelper.GetMovieList(IOHelper.CurrentFolder);
			var movieWindow = new MovieListWindow(this._fileHelper) { WindowStyle = this.WindowStyle };

			var result = movieWindow.ShowDialog();

			if (result == true)
			{
				this.PlayMovie(movieWindow.Movie);
			}
		}

		public void Play()
		{
			this.InternalPlay();
		}

		public void Pause()
		{
			this.PlayButton.Content = SetButtonContent("PlayImage");
			this.Player.Pause();

			this._isNowPlaying = false;
			PlatformInvoke.SetThreadExecutionState(ExecutionState.ES_CONTINUOUS);
		}

		public void Skip()
		{
			if (!this.Player.HasVideo)
			{
				return;
			}

			var pos = this.Player.MediaPosition + TimeSpan.FromSeconds(10).Ticks;

			if (pos > this.Player.MediaDuration)
			{
				pos = this.Player.MediaDuration;
			}

			this.Player.MediaPosition = pos;
		}

		public void SkipBack()
		{
			if (!this.Player.HasVideo)
			{
				return;
			}

			var pos = this.Player.MediaPosition - TimeSpan.FromSeconds(10).Ticks;

			if (pos < 0)
			{
				pos = 0;
			}

			this.Player.MediaPosition = pos;
		}

		public void FastForward()
		{
			this.Wind(true);
		}

		public void Rewind()
		{
			this.Wind(false);
		}

		public void ChangeScreenState()
		{
			if (this.WindowState == WindowState.Maximized && this.WindowStyle == WindowStyle.None)
			{
				this.RestoreScreen();
			}
			else
			{
				this.FullScreen();
			}
		}

		public void Mute()
		{
			this.MuteButton.Content = SetButtonContent(this.IsMuted ? "VolumeImage" : "MutedImage");

			this.IsMuted = !this.IsMuted;
		}

		public void VolumeDown()
		{
			this.LastVolume -= 0.02;

			if (this.LastVolume < MIN_VOLUME)
			{
				this.LastVolume = MIN_VOLUME;
			}

			this.Player.Volume = this.LastVolume;
			this.ShowVolumeValue();
		}

		public void VolumeUp()
		{
			this.LastVolume += 0.02;

			if (this.LastVolume > MAX_VOLUME)
			{
				this.LastVolume = MAX_VOLUME;
			}

			this.Player.Volume = this.LastVolume;
			this.ShowVolumeValue();
		}

		public void Stop()
		{
			this._isNowPlaying = false;
			this.Stopped = true;

			this.SaveMoviePosition();

			this._fastPlayTimer.Stop();
			this.Player.Stop();
			
			this.PlayButton.Content = SetButtonContent("PlayImage");
			this.SetControlsEnabled(false);
			PlatformInvoke.SetThreadExecutionState(ExecutionState.ES_CONTINUOUS);
		}

		public void Next()
		{
			this.PlayNextFile(true);
		}

		public void Previous()
		{
			this.PlayNextFile(false);
		}

		public void Suspend()
		{
			var wnd = new SuspendWindow();

			var dlg = wnd.ShowDialog();

			if (dlg == null)
			{
				return;
			}

			try
			{
				switch (wnd.SuspendType)
				{
					case SuspendType.Shutdown:
						PlatformInvoke.ExitWindowsEx(ExitWindows.ShutDown | ExitWindows.ForceIfHung, 0);
						return;
					case SuspendType.Hibernate:
						PlatformInvoke.SetSuspendState(true, true, true);
						return;
					default:
						return;
				}
			}
			catch (Exception ex)
			{
				App.WriteLog(ex);
			}
		}

		#endregion
	}
}
