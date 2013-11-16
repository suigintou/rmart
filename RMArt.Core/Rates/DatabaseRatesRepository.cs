using LinqToDB;
using System.Collections.Generic;
using System.Linq;

namespace RMArt.Core
{
	public class DatabaseRatesRepository : IRatesRepository
	{
		private readonly IDataContextProvider _dataContextProvider;

		public DatabaseRatesRepository(IDataContextProvider dataContextProvider)
		{
			_dataContextProvider = dataContextProvider;
		}

		public IList<Rate> List(RatesQuery predicate, int? skipCount, int? takeCount)
		{
			using (var context = _dataContextProvider.CreateDataContext())
			{
				var rates = Filter(context.Rates(), predicate).OrderByDescending(r => r.Date).AsQueryable();

				if (skipCount != null && skipCount > 0)
					rates = rates.Skip(skipCount.Value);
				if (takeCount != null)
					rates = rates.Take(takeCount.Value);

				return rates.ToArray();
			}
		}

		public int Count(RatesQuery predicate)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return Filter(context.Rates(), predicate).Count();
		}

		public bool IsExists(RatesQuery predicate)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return Filter(context.Rates(), predicate).Any();
		}

		public void Add(Rate rate)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				context
					.Rates()
					.Value(_ => _.PictureID, rate.PictureID)
					.Value(_ => _.UserID, rate.UserID)
					.Value(_ => _.Score, rate.Score)
					.Value(_ => _.Date, rate.Date)
					.Value(_ => _.IPAddress, rate.IPAddress)
					.Value(_ => _.IsActive, rate.IsActive)
					.Insert();
		}

		public int Delete(RatesQuery predicate)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return Filter(context.Rates(), predicate).Delete();
		}

		public int UpdateIsActive(RatesQuery predicate, bool isActive)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return Filter(context.Rates(), predicate).Set(_ => _.IsActive, isActive).Update();
		}

		private static IQueryable<Rate> Filter(IQueryable<Rate> source, RatesQuery query)
		{
			var predicate = PredicateBuilder.True<Rate>();

			if (query.PictureID != null)
				predicate = predicate.And(r => r.PictureID == query.PictureID);
			if (query.UserID != null)
				predicate = predicate.And(r => r.UserID == query.UserID);
			if (query.Score != null)
				predicate = predicate.And(r => r.Score == query.Score);
			if (query.IsActive != null)
				predicate = predicate.And(r => r.IsActive == query.IsActive);

			return source.Where(predicate);
		}
	}
}