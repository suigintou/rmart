using System.Configuration;

namespace RMArt.Web
{
	[ConfigurationCollection(typeof(string), CollectionType = ConfigurationElementCollectionType.AddRemoveClearMap)]
	public class CulturesCollection : ConfigurationElementCollection
	{
		protected override ConfigurationElement CreateNewElement()
		{
			return new CultureConfigurationElement();
		}

		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((CultureConfigurationElement)element).Name;
		}
	}
}