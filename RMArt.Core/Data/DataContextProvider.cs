using LinqToDB;

namespace RMArt.Core
{
	public class DataContextProvider : IDataContextProvider
	{
		private readonly string _configurationString;

		public DataContextProvider(string configurationString)
		{
			_configurationString = configurationString;
		}

		public DataContext CreateDataContext()
		{
			return new DataContext(_configurationString);
		}
	}
}