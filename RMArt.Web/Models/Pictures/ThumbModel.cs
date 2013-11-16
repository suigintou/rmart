using System.Collections.Generic;
using RMArt.Core;

namespace RMArt.Web.Models
{
	public class ThumbModel
	{
		public int ID { get; set; }
		public ModerationStatus Status { get; set; }
		public Rating Rating { get; set; }
		public bool RequiresTagging { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int FileSize { get; set; }
		public ImageFormat Format { get; set; }
		public int Score { get; set; }
		public int RatesCount { get; set; }
		public IList<Tag> Tags { get; set; }
	}
}