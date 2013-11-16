namespace RMArt.Core
{
	public static class HistoryHelper
	{
		public static void LogValueChange(
			this IHistoryService historyService,
			ObjectReference target,
			HistoryField field,
			object oldValue,
			object newValue,
			Identity identity,
			string comment)
		{
			historyService.Log(
				target,
				field,
				oldValue != null ? oldValue.ToString() : null,
				newValue != null ? newValue.ToString() : null,
				identity,
				comment);
		}

		public static void LogCollectionAdd(
			this IHistoryService historyService,
			ObjectReference target,
			HistoryField field,
			object value,
			Identity identity,
			string comment)
		{
			historyService.LogValueChange(target, field, null, value, identity, comment);
		}

		public static void LogCollectionRemove(
			this IHistoryService historyService,
			ObjectReference target,
			HistoryField field,
			object value,
			Identity identity,
			string comment)
		{
			historyService.LogValueChange(target, field, value, null, identity, comment);
		}

		//public static void Rollback(this HistoryEvent e, IPicturesService picturesService, ITagsService tagsService, Identity identity)
		//{
		//    switch (e.Target.Type)
		//    {
		//        case ObjectType.Picture:
		//            switch (e.Field)
		//            {
		//                case HistoryField.Status:
		//                    picturesService.SetStatus(e.Target.ID, e.OldValue.ParseEnum<ModerationStatus>(), identity);
		//                    break;
		//                case HistoryField.Rating:
		//                    picturesService.SetRating(e.Target.ID, e.OldValue.ParseEnum<Rating>(), identity);
		//                    break;
		//                case HistoryField.RequiresTagging:
		//                    picturesService.SetRequiresTagging(e.Target.ID, e.OldValue.ParseBoolean(), identity);
		//                    break;
		//                case HistoryField.Tags:
		//                    if (!string.IsNullOrEmpty(e.NewValue))
		//                        picturesService.UnassignTag(e.Target.ID, e.NewValue.ParseInt32(), identity);
		//                    else
		//                        picturesService.AssignTag(e.Target.ID, e.OldValue.ParseInt32(), identity);
		//                    break;
		//                default:
		//                    throw new NotSupportedException(string.Format("Not supported picture field \"{0}\".", e.Field));
		//            }
		//            break;

		//        case ObjectType.Tag:
		//            switch (e.Field)
		//            {
		//                case HistoryField.Status:
		//                    tagsService.SetStatus(e.Target.ID, e.OldValue.ParseEnum<ModerationStatus>(), identity);
		//                    break;
		//                case HistoryField.Name:
		//                    tagsService.SetName(e.Target.ID, e.OldValue, identity);
		//                    break;
		//                case HistoryField.Type:
		//                    tagsService.SetType(e.Target.ID, e.OldValue.ParseEnum<TagType>(), identity);
		//                    break;
		//                case HistoryField.Aliases:
		//                    if (!string.IsNullOrEmpty(e.NewValue))
		//                        tagsService.DeleteAlias(e.Target.ID, e.NewValue, identity);
		//                    else
		//                        tagsService.CreateAlias(e.Target.ID, e.OldValue, identity);
		//                    break;
		//                case HistoryField.Parents:
		//                    if (!string.IsNullOrEmpty(e.NewValue))
		//                        tagsService.CreateRelation(e.NewValue.ParseInt32(), e.Target.ID, identity);
		//                    else
		//                        tagsService.DeleteRelation(e.OldValue.ParseInt32(), e.Target.ID, identity);
		//                    break;
		//                default:
		//                    throw new NotSupportedException(string.Format("Not supported tag field \"{0}\".", e.Field));
		//            }
		//            break;

		//        default:
		//            throw new NotSupportedException(string.Format("Not supported object \"{0}\".", e.Target.Type));
		//    }
		//}

		public static bool IsCollection(this HistoryField field)
		{
			switch (field)
			{
				case HistoryField.Tags:
				case HistoryField.Aliases:
				case HistoryField.Parents:
					return true;
				default:
					return false;
			}
		}
	}
}