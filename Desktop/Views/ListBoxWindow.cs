using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace RemoteVideoPlayer.Views
{
	public abstract class ListBoxWindow : Window, IListBoxWindow
	{
		protected abstract ListBox ListBox { get; }

		protected ListBoxWindow()
		{
			this.Loaded += this.ListBoxWindowLoaded;
			this.Closed += ListBoxWindowClosed;
		}

		private void ListBoxWindowLoaded(object sender, RoutedEventArgs e)
		{
			PlayerServiceManager.AddFacility(this);

			this.Activate();
			this.Topmost = true;
			this.Topmost = false;
			this.Focus();

			this.ListBox.SelectionChanged += this.ListBoxSelectionChanged;
		}

		private void ListBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			this.FocusSelectedItem();
		}

		private static void ListBoxWindowClosed(object sender, System.EventArgs e)
		{
			PlayerServiceManager.RemoveLastFacility();
		}

		private ScrollViewer GetScrollViewer(DependencyObject obj)
		{
			for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
			{
				var child = VisualTreeHelper.GetChild(obj, i);

				if (child is ScrollViewer)
				{
					return (ScrollViewer)child;
				}

				return this.GetScrollViewer(child);
			}

			return null;
		}

		#region Implementation of IListBoxWindow

		public void Next()
		{
			this.WindowDispatcher?.Invoke(this.InternalNext, DispatcherPriority.Normal);
		}

		public void Previous()
		{
			this.WindowDispatcher?.Invoke(this.InternalPrevious, DispatcherPriority.Normal);
		}

		public void SelectItem()
		{
			this.WindowDispatcher?.Invoke(this.InternalSelectItem, DispatcherPriority.Normal);
		}

		public virtual void LevelUp() { }

		public virtual void PageUp()
		{
			var viewer = this.GetScrollViewer(this.ListBox);

			var i = (int)(viewer.VerticalOffset - viewer.ViewportHeight);

			if (i < 0)
			{
				i = 0;
			}

			viewer.ScrollToVerticalOffset(i);
			this.ListBox.SelectedIndex = i;
		}

		public virtual void PageDown()
		{
			var viewer = this.GetScrollViewer(this.ListBox);

			var i = (int)(viewer.VerticalOffset + viewer.ViewportHeight);

			if (i >= this.ListBox.Items.Count)
			{
				i = this.ListBox.Items.Count - 1;
			}

			viewer.ScrollToVerticalOffset(i);
			this.ListBox.SelectedIndex = i;
		}

		#endregion

		#region Implementation of IPlayer

		public abstract Dispatcher WindowDispatcher { get; }

		#endregion

		protected virtual void InternalSelectItem() { }

		protected void FocusSelectedItem()
		{
			this.ListBox.ScrollIntoView(this.ListBox.SelectedItem);

			var listBoxItem = (ListBoxItem)this.ListBox.ItemContainerGenerator.ContainerFromItem(this.ListBox.SelectedItem);
			listBoxItem?.Focus();
		}

		private void InternalPrevious()
		{
			var index = this.ListBox.SelectedIndex;

			index--;

			if (index < 0)
			{
				index = this.ListBox.Items.Count - 1;
			}

			this.ListBox.SelectedIndex = index;
		}

		private void InternalNext()
		{
			var index = this.ListBox.SelectedIndex;

			index++;

			if (index == this.ListBox.Items.Count)
			{
				index = 0;
			}

			this.ListBox.SelectedIndex = index;
		}
	}
}