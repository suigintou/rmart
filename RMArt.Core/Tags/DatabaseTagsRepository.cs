using System;
using System.Data;
using System.Linq;
using LinqToDB;

namespace RMArt.Core
{
	public class TagsRepository : ITagsRepository
	{
		private readonly IDataContextProvider _dataContextProvider;

		public TagsRepository(IDataContextProvider dataContextProvider)
		{
			_dataContextProvider = dataContextProvider;
		}

		public IQueryable<Tag> Find(TagsQuery query = null)
		{
			return Filter(_dataContextProvider.CreateDataContext().Tags(), query);
		}

		public Tag LoadTag(int id)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return LoadTagInternal(context, id);
		}

		public int Add(Tag tag)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return
					Convert.ToInt32(
						context
							.Tags()
								.Value(_ => _.Name, tag.Name)
								.Value(_ => _.Status, tag.Status)
								.Value(_ => _.Type, tag.Type)
								.Value(_ => _.CreatorID, tag.CreatorID)
								.Value(_ => _.CreatorIP, tag.CreatorIP)
								.Value(_ => _.CreationDate, tag.CreationDate)
								.Value(_ => _.UsageCount, tag.UsageCount)
							.InsertWithIdentity());
		}

		public Tag Update(int id, TagUpdate update)
		{
			using (var context = _dataContextProvider.CreateDataContext())
			using (var transaction = new DataContextTransaction(context))
			{
				transaction.BeginTransaction(IsolationLevel.Serializable);

				var oldTag = LoadTagInternal(context, id);

				if (oldTag != null)
				{
					var source = context.Tags().Where(_ => _.ID == id);
					if (update.Name != null)
						source.Set(_ => _.Name, update.Name).Update();
					if (update.Status != null)
						source.Set(_ => _.Status, update.Status).Update();
					if (update.Type != null)
						source.Set(_ => _.Type, update.Type).Update();
					if (update.AddParents != null)
						foreach (var parentID in update.AddParents)
							context.TagRelations().Value(_ => _.ParentID, parentID).Value(_ => _.ChildrenID, id).Insert();
					if (update.RemoveParents != null)
						foreach (var parentID in update.RemoveParents)
							context.TagRelations().Delete(_ => _.ParentID == parentID && _.ChildrenID == id);
					if (update.AddAliases != null)
						foreach (var alias in update.AddAliases)
							context.TagAliases().Value(_ => _.TagID, id).Value(_ => _.Name, alias).Insert();
					if (update.RemoveAliases != null)
						foreach (var alias in update.RemoveAliases)
							context.TagAliases().Delete(_ => _.TagID == id && _.Name == alias);
				}

				transaction.CommitTransaction();

				return oldTag;
			}
		}

		public void Remove(int id)
		{
			using (var context = _dataContextProvider.CreateDataContext())
			using (var transaction = new DataContextTransaction(context))
			{
				transaction.BeginTransaction(IsolationLevel.Serializable);

				context.TagRelations().Delete(_ => _.ChildrenID == id);
				context.TagRelations().Delete(_ => _.ParentID == id);
				context.TagAliases().Delete(_ => _.TagID == id);
				context.Tags().Delete(t => t.ID == id);

				transaction.CommitTransaction();
			}
		}

		public void IncrementUsageCount(int id, int value)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				context
					.Tags()
					.Where(t => t.ID == id)
					.Set(_ => _.UsageCount, t => t.UsageCount + value)
					.Update();
		}

		public void SetUsageCount(int id, int value)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				context
					.Tags()
					.Where(t => t.ID == id)
					.Set(_ => _.UsageCount, value)
					.Update();
		}

		private static Tag LoadTagInternal(IDataContext context, int id)
		{
			var tag = context.Tags().SingleOrDefault(_ => _.ID == id);
			if (tag == null)
				return null;

			tag.AliasNames = context.TagAliases().Where(_ => _.TagID == id).Select(_ => _.Name).ToArray();
			tag.ParentIDs = context.TagRelations().Where(_ => _.ChildrenID == id).Select(_ => _.ParentID).ToArray();
			tag.ChildrenIDs = context.TagRelations().Where(_ => _.ParentID == id).Select(_ => _.ChildrenID).ToArray();

			return tag;
		}

		private static IQueryable<Tag> Filter(IQueryable<Tag> tags, TagsQuery query)
		{
			if (query == null)
				return tags;

			var predicate = PredicateBuilder.True<Tag>();
			if (query.AllowedStatuses != null)
				predicate = predicate.And(query.AllowedStatuses.Aggregate(PredicateBuilder.False<Tag>(), (pred, s) => pred.Or(t => t.Status == s)));
			if (query.Name != null)
				predicate = predicate.And(t => t.Name == query.Name || t.Aliases.Any(_ => _.Name == query.Name));
			if (!string.IsNullOrEmpty(query.Term))
				predicate = predicate.And(t => t.Name.Contains(query.Term) || t.Aliases.Any(_ => _.Name.Contains(query.Term)));
			if (query.Type != null)
				predicate = predicate.And(t => t.Type == query.Type);
			tags = tags.Where(predicate);

			switch (query.SortBy)
			{
				case TagsSortOrder.None:
					break;
				case TagsSortOrder.NameAsc:
					tags = tags.OrderBy(t => t.Name);
					break;
				case TagsSortOrder.NameDesc:
					tags = tags.OrderByDescending(t => t.Name);
					break;
				case TagsSortOrder.Oldest:
					tags = tags.OrderBy(t => t.CreationDate).ThenBy(t => t.ID);
					break;
				case TagsSortOrder.Newest:
					tags = tags.OrderByDescending(t => t.CreationDate).ThenByDescending(t => t.ID);
					break;
				case TagsSortOrder.LeastUsed:
					tags = tags.OrderBy(t => t.UsageCount);
					break;
				case TagsSortOrder.MostUsed:
					tags = tags.OrderByDescending(t => t.UsageCount);
					break;
				default:
					throw new NotSupportedException();
			}

			if (query.Skip != null)
				tags = tags.Skip(query.Skip.Value);
			if (query.Take != null)
				tags = tags.Take(query.Take.Value);

			return tags;
		}
	}
}