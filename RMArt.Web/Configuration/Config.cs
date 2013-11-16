using System.Configuration;

namespace RMArt.Web
{
	public static class Config
	{
		private static readonly ConfigSection _config = (ConfigSection)ConfigurationManager.GetSection("rmart");

		public static ConfigSection Default
		{
			get { return _config; }
		}
	}
}