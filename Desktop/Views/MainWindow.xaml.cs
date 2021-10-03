﻿using System;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using RemoteVideoPlayer.Helpers;
using RemoteVideoPlayer.Models;
using WPFMediaKit.DirectShow.MediaPlayers;

namespace RemoteVideoPlayer.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow
	{
		private readonly DispatcherTimer _mouseMoveTimer;

		private readonly DispatcherTimer _volumeTextTimer;

		private readonly DispatcherTimer _fastPlayTimer;

		private Cursor _cursor;

		public MainWindow()
		{
			InitializeComponent();

			this._mouseMoveTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(3) };
			this._mouseMoveTimer.Tick += this.MouseMoveTimerTick;

			this._volumeTextTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
			this._volumeTextTimer.Tick += this.VolumeTextTimerTick;

			this._fastPlayTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
			this._fastPlayTimer.Tick += this.FastPlayTimerTick;

			this._fileHelper = new IOHelper();

			this.LastVolume = ConfigHelper.LastVolume;

			this.Loaded += this.MainWindowLoaded;
			this.Closing += this.MainWindowClosing;
			this.MouseMove += this.MainWindowMouseMove;
			this.PreviewMouseDown += this.MainWindowPreviewMouseDown;
			this.PreviewKeyUp += this.MainWindowPreviewKeyUp;
		}

		private void MainWindowPreviewMouseDown(object sender, MouseButtonEventArgs e)
		{
			this.ShowRoot();
		}

		private void MainWindowPreviewKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F11:
                    this.FullScreen();
                    break;
                case Key.Escape:
                    this.RestoreScreen();
                    break;
                case Key.MediaPlayPause:
                case Key.Space:
                    this.PlayPause();
                    break;
                case Key.Play:
					this.Play();
                    break;
				case Key.Pause:
					this.Pause();
                    break;
                case Key.VolumeMute:
                    this.Mute();
                    break;
                case Key.VolumeDown:
                    this.VolumeDown();
                    break;
                case Key.VolumeUp:
                    this.VolumeUp();
                    break;
                case Key.MediaNextTrack:
                    this.Next();
                    break;
                case Key.MediaPreviousTrack:
                    this.Previous();
                    break;
                case Key.MediaStop:
                    this.Stop();
                    break;
                case Key.Enter:
                case Key.SelectMedia:
                case Key.Apps:
                    this.SelectMovie();
                    break;
                case Key.Left:
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        this.Previous();
                    }
					else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        this.Rewind();
                    }
                    else
                    {
                        this.SkipBack();
                    }
                    break;
                case Key.Right:
                    if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
                    {
                        this.Next();
                    }
					else if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
                    {
                        this.FastForward();
                    }
                    else
                    {
                        this.Skip();
                    }
                    break;
                default:
                    System.Diagnostics.Debug.WriteLine(e.Key.ToString());
                    break;
            }

            this.ShowRoot();
        }

        private void MouseMoveTimerTick(object sender, EventArgs e)
		{
			this.Root.Opacity = 0;
			this.Cursor = Cursors.None;
			this._mouseMoveTimer.Stop();
		}

		private void VolumeTextTimerTick(object sender, EventArgs e)
		{
			this.VolumeValueBlock.Opacity = 0;
			this._volumeTextTimer.Stop();
		}

		private void MainWindowClosing(object sender, CancelEventArgs e)
		{
			this.Stop();
			ConfigHelper.LastVolume = this.LastVolume;
		}

		private void MainWindowLoaded(object sender, RoutedEventArgs e)
		{
			this._cursor = this.Cursor;

			if (App.Args.Count == 1)
			{
				this.OpenFile(App.Args[0]);
			}

			this.SizeChanged += this.MainWindowSizeChanged;

			this.VolumeValueBlock.DataContext = this;

			PlayerServiceManager.AddFacility(this);

			this.Activate();
			this.Topmost = true;
			this.Topmost = false;

			this.SelectMovie();
		}

		private void MainWindowSizeChanged(object sender, SizeChangedEventArgs e)
		{
			this.Resize();
			this.ChangeLocation();
		}

		private void ChangeLocation()
		{
			this.MainForm.Left = (SystemParameters.WorkArea.Width - this.MainForm.ActualWidth) / 2;
			this.MainForm.Top = (SystemParameters.WorkArea.Height - this.MainForm.ActualHeight) / 2;
		}

		private void PlayVideo(string file)
		{
			if (String.IsNullOrEmpty(file))
			{
				return;
			}

			var movie = new Movie(file);

			var savedMovie = this._fileHelper.GetSavedMovie(file);

			if (savedMovie != null && savedMovie.Position > TimeSpan.Zero)
			{
				var pos = SelectMoviePositionWindow.DefineMoviePosition(savedMovie);

				if (pos.Span == null)
				{
					return;
				}

				movie.Position = pos.Span.Value;
			}

			this.PlayMovie(movie);
		}

		private void PlayButtonClick(object sender, RoutedEventArgs e)
		{
			this.PlayPause();
		}

		private void Player_MediaOpened(object sender, RoutedEventArgs e)
		{
			this.UpdatePlayerParameters();
		}

		private void MainWindowMouseMove(object sender, MouseEventArgs e)
		{
			this.ShowRoot();
		}

		public void ShowRoot()
		{
			this._mouseMoveTimer.Stop();
			this.Cursor = this._cursor;
			this.Root.Opacity = 1;
			this._mouseMoveTimer.Start();
		}

		private void PreviousButtonClick(object sender, RoutedEventArgs e)
		{
			this.Previous();
		}

		private void RewindButtonClick(object sender, RoutedEventArgs e)
		{
			this.Rewind();
		}

		private void ForwardButtonClick(object sender, RoutedEventArgs e)
		{
			this.FastForward();
		}

		private void NextButtonClick(object sender, RoutedEventArgs e)
		{
			this.Next();
		}

		private void VolumeDownButtonClick(object sender, RoutedEventArgs e)
		{
			this.VolumeDown();
		}

		private void ShowVolumeValue()
		{
			this._volumeTextTimer.Stop();
			this.VolumeValueBlock.Opacity = 1;
			this._volumeTextTimer.Start();
		}

		private void MuteButtonClick(object sender, RoutedEventArgs e)
		{
			this.Mute();
		}

		private void VolumeUpButtonClick(object sender, RoutedEventArgs e)
		{
			this.VolumeUp();
		}

		private void ScreenButtonClick(object sender, RoutedEventArgs e)
		{
			this.ChangeScreenState();
		}

		private void OpenFile(string file)
		{
			this.PlayVideo(file);
		}

		private void Player_MediaEnded(object sender, RoutedEventArgs e)
		{
			this.Next();
		}

		private void FolderButtonClick(object sender, RoutedEventArgs e)
		{
			this.SelectMovie();
		}

		private void SkipButtonClick(object sender, RoutedEventArgs e)
		{
			this.Skip();
		}

		private void SkipBackButtonClick(object sender, RoutedEventArgs e)
		{
			this.SkipBack();
		}

		private void Player_MediaFailed(object sender, MediaFailedEventArgs e)
		{
			this.Dispatcher.Invoke(() => this.InfoBlock.Text = e.Exception.Message);
		}

		private static Image SetButtonContent(string resourcePath)
		{
			return new Image
			{
				Source = Application.Current.Resources[resourcePath] as BitmapImage,
				Style = Application.Current.Resources["ImageStyle"] as Style
			};
		}

		private void FastPlayTimerTick(object sender, EventArgs e)
		{
			if (!this.Player.HasVideo || !(this._fastPlayTimer.Tag is bool))
			{
				return;
			}
			
			var forward = (bool)this._fastPlayTimer.Tag;

			if (forward)
			{
				this.IncreasePosition();
			}
			else
			{
				this.DecreasePosition();
			}
		}

		private void SetControlsEnabled(bool enabled)
		{
			this.ProgressSlider.IsEnabled = enabled;
			this.PreviousButton.IsEnabled = enabled;
			this.NextButton.IsEnabled = enabled;
			this.VolumeDownButton.IsEnabled = enabled;
			this.MuteButton.IsEnabled = enabled;
			this.VolumeUpButton.IsEnabled = enabled;

			if (this.Player.Source != null)
			{
				this.SkipBackButton.IsEnabled = true;
				this.SkipButton.IsEnabled = true;
				this.RewindButton.IsEnabled = true;
				this.ForwardButton.IsEnabled = true;
			}
		}

		#region Implementation of IPlayer

		public Dispatcher WindowDispatcher => this.Dispatcher;

		#endregion
	}
}
