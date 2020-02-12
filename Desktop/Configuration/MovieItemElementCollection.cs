using System.Collections.Generic;
using System.Configuration;

namespace RemoteVideoPlayer.Configuration
{
	[ConfigurationCollection(
		typeof(MovieItemElementCollection),
		AddItemName = "folder",
		CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class MovieItemElementCollection : ConfigurationElementCollection, IPlayerConfigurationElementCollection 
	{
		#region Overrides of ConfigurationElementCollection

		/// <summary>When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.</summary>
		/// <returns>A newly created <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new MovieItemElement();
		}

		/// <summary>Gets the element key for a specified configuration element when overridden in a derived class.</summary>
		/// <returns>An <see cref="T:System.Object" /> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to return the key for. </param>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((MovieItemElement)element).Path;
		}

		/// <summary>Sets the <see cref="M:System.Configuration.ConfigurationElementCollection.IsReadOnly" /> property for the <see cref="T:System.Configuration.ConfigurationElementCollection" /> object and for all sub-elements.</summary>
		protected override void SetReadOnly() { }

		#endregion

		public void Clear()
		{
			this.BaseClear();
		}

		public virtual void AddRange<T>(IEnumerable<T> elements) where T : MovieItemElement
		{
			foreach (var element in elements)
			{
				this.BaseAdd(element);
			}
		}
	}
}