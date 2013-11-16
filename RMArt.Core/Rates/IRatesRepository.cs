using System.Collections.Generic;

namespace RMArt.Core
{
	public interface IRatesRepository
	{
		IList<Rate> List(RatesQuery query = null, int? skipCount = null, int? takeCount = null);
		int Count(RatesQuery predicate = null);
		bool IsExists(RatesQuery predicate = null);
		void Add(Rate rate);
		int Delete(RatesQuery predicate);
		int UpdateIsActive(RatesQuery predicate, bool isActive);
	}
}