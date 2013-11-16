using LinqToDB;

namespace RMArt.Core
{
	public interface IDataContextProvider
	{
		DataContext CreateDataContext();
	}
}