using System.Configuration;

namespace RMArt.Web
{
	public class CultureConfigurationElement : ConfigurationElement
	{
		[ConfigurationProperty("name")]
		public string Name
		{
			get { return (string)this["name"]; }
			set { this["name"] = value; }
		}
	}
}