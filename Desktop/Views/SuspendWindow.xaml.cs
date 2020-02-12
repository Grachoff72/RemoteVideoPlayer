using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace RemoteVideoPlayer.Views
{
	/// <summary>
	/// Interaction logic for SuspendWindow.xaml
	/// </summary>
	public partial class SuspendWindow
	{
		public SuspendType SuspendType { get; private set; }

		public SuspendWindow()
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

		private void ShutdownButtonClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.SuspendType = SuspendType.Shutdown;
			this.Close();
		}

		private void HibernateButtonClick(object sender, RoutedEventArgs e)
		{
			this.DialogResult = true;
			this.SuspendType = SuspendType.Hibernate;
			this.Close();
		}

		private void CloseButtonClick(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		#region Overrides of ListBoxWindow

		protected override ListBox ListBox => this.SelectListBox;

		public override Dispatcher WindowDispatcher => this.Dispatcher;

		protected override void InternalSelectItem()
		{
			if (this.ShutdownItem.IsSelected)
			{
				this.ShutdownButtonClick(null, null);
			}
			else if (this.HibernateItem.IsSelected)
			{
				this.HibernateButtonClick(null, null);
			}
			else
			{
				this.CloseButtonClick(null, null);
			}
		}

		#endregion
	}
}
