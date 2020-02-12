using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using RemoteVideoPlayer.Models;

namespace RemoteVideoPlayer.Views
{
    /// <summary>
    /// Interaction logic for SelectMoviePositionWindow.xaml
    /// </summary>
    public partial class SelectMoviePositionWindow
    {
	    private PlayType PlayType { get; set; } = PlayType.Continue;

		public SelectMoviePositionWindow()
        {
            InitializeComponent();

			this.PreviewKeyUp += this.SelectMoviePositionWindowPreviewKeyUp;
        }

	    private void SelectMoviePositionWindowPreviewKeyUp(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				this.InternalSelectItem();
			}
		}

	    private void BeginButtonClick(object sender, RoutedEventArgs e)
	    {
		    this.DialogResult = true;
			this.PlayType = PlayType.Begin;
			this.Close();
	    }

	    private void ContinueButtonClick(object sender, RoutedEventArgs e)
	    {
		    this.DialogResult = true;
		    this.Close();
	    }

	    private void CloseButtonClick(object sender, RoutedEventArgs e)
	    {
		    this.Close();
	    }

	    internal static MoviePosition DefineMoviePosition(Movie movie)
	    {
			var selectPositionWnd = new SelectMoviePositionWindow();

			var result = selectPositionWnd.ShowDialog();

		    var pos = new MoviePosition();

			if (result != true)
			{
				return pos;
			}

		    pos.Span = selectPositionWnd.PlayType == PlayType.Continue ? movie.Position : TimeSpan.Zero;

		    return pos;
	    }

		#region Overrides of ListBoxWindow

	    protected override ListBox ListBox => this.SelectListBox;

	    public override Dispatcher WindowDispatcher => this.Dispatcher;

		protected override void InternalSelectItem()
		{
			if (this.BeginItem.IsSelected)
			{
				this.BeginButtonClick(null, null);
			}
			else if (this.ContinueItem.IsSelected)
			{
				this.ContinueButtonClick(null, null);
			}
			else
			{
				this.CloseButtonClick(null, null);
			}
		}

		#endregion
    }
}
