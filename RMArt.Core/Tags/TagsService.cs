using System;
using System.Linq;

namespace RMArt.Core
{
	public class TagsService : ITagsService
	{
		private readonly ITagsRepository _tagsRepository;
		private readonly IHistoryService _historyService;

		public TagsService(ITagsRepository tagsRepository, IHistoryService historyService)
		{
			_tagsRepository = tagsRepository;
			_historyService = historyService;
		}

		public IQueryable<Tag> Find(TagsQuery query = null)
		{
			return _tagsRepository.Find(query);
		}

		public Tag LoadTag(int id)
		{
			return _tagsRepository.LoadTag(id);
		}

		public int Create(string name, TagType type, ModerationStatus status, Identity identity)
		{
			if (!TagsHelper.IsValidTagName(name))
				throw new ArgumentException("Invalid name.", "name");
			if (_tagsRepository.Find(new TagsQuery { Name = name }).Any())
				throw new ArgumentException("Name already taken.", "name");

			return _tagsRepository.Add(
				new Tag
				{
					Name = name,
					Status = status,
					Type = type,
					CreatorID = identity.UserID,
					CreatorIP = identity.IPAddress.GetAddressBytes(),
					CreationDate = DateTime.UtcNow
				});
		}

		public void Update(int id, TagUpdate update, Identity identity, string comment = null)
		{
			if (update.Name != null)
			{
				if (!TagsHelper.IsValidTagName(update.Name))
					throw new ArgumentException("Invalid name.", "update");
				var existingID = this.GetIDByName(update.Name);
				if (existingID != null && existingID.Value != id)
					throw new ArgumentException("Name already taken.", "update");
			}

			if (update.AddAliases != null)
				foreach (var alias in update.AddAliases)
				{
					if (!TagsHelper.IsValidTagName(alias))
						throw new ArgumentException("Invalid alias name.", "update");
					if (_tagsRepository.Find(new TagsQuery { Name = alias }).Any())
						throw new ArgumentException("Name already taken.", "update");
				}

			if (update.AddParents != null)
				foreach (var parentID in update.AddParents)
				{
					if (LoadTag(parentID) == null)
						throw new ArgumentException("Parent tag does not exists.", "update");
					if (this.IsChildren(id, parentID, true))
						throw new InvalidOperationException("Parent tag is children of children tag.");
				}

			var old = _tagsRepository.Update(id, update);
			if (old == null)
				return;

			if (update.Name != null && !update.Name.Equals(old.Name, StringComparison.Ordinal))
				_historyService.LogValueChange(new ObjectReference(ObjectType.Tag, id), HistoryField.Name, old.Name, update.Name, identity, comment);

			if (update.Status != null && update.Status.Value != old.Status)
				_historyService.LogValueChange(new ObjectReference(ObjectType.Tag, id), HistoryField.Status, (int)old.Status, (int)update.Status, identity, comment);

			if (update.Type != null && update.Type.Value != old.Type)
				_historyService.LogValueChange(new ObjectReference(ObjectType.Tag, id), HistoryField.Type, (int)old.Type, (int)update.Type, identity, comment);

			if (update.AddAliases != null)
				foreach (var alias in update.AddAliases.Except(old.AliasNames).Distinct())
					_historyService.LogCollectionAdd(new ObjectReference(ObjectType.Tag, id), HistoryField.Aliases, alias, identity, comment);
			if (update.RemoveAliases != null)
				foreach (var alias in update.RemoveAliases.Intersect(old.AliasNames).Distinct())
					_historyService.LogCollectionRemove(new ObjectReference(ObjectType.Tag, id), HistoryField.Aliases, alias, identity, comment);

			if (update.AddParents != null)
				foreach (var parentID in update.AddParents.Except(old.ParentIDs).Distinct())
					_historyService.LogCollectionAdd(new ObjectReference(ObjectType.Tag, id), HistoryField.Parents, parentID, identity, comment);
			if (update.RemoveParents != null)
				foreach (var parentID in update.RemoveParents.Intersect(old.ParentIDs).Distinct())
					_historyService.LogCollectionRemove(new ObjectReference(ObjectType.Tag, id), HistoryField.Parents, parentID, identity, comment);
		}

		public void Delete(int id)
		{
			var tag = LoadTag(id);
			if (tag == null)
				return;

			if (tag.UsageCount > 0)
				throw new InvalidOperationException("Can not remove used tag.");

			_tagsRepository.Remove(id);

			_historyService.DeleteFor(new ObjectReference(ObjectType.Tag, id));
		}

		public void IncrementUsageCount(int id, int value)
		{
			_tagsRepository.IncrementUsageCount(id, value);
		}

		public void SetUsageCount(int id, int value)
		{
			_tagsRepository.SetUsageCount(id, value);
		}
	}
}