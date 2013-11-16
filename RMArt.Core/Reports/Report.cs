using System;
using LinqToDB.Mapping;

namespace RMArt.Core
{
	[Table("Reports")]
	public class Report
	{
		[Identity, PrimaryKey]
		public int ID { get; set; }

		[Column]
		public ObjectType? TargetType { get; set; }

		[Column]
		public int? TargetID { get; set; }

		[Column]
		public ReportType ReportType { get; set; }

		[Column]
		public string Message { get; set; }

		[Column]
		public int? CreatedBy { get; set; }

		[Column]
		public DateTime CreatedAt { get; set; }

		[Column, NotNull]
		public byte[] CreatorIP { get; set; }

		[Column]
		public bool IsResolved { get; set; }

		[Column]
		public int? ResolvedBy { get; set; }

		[Column]
		public DateTime? ResolvedAt { get; set; }

		[Column]
		public byte[] ResolverIP { get; set; }
	}
}