using RMArt.Core;

namespace RMArt.Web.Models
{
	public class TagModel
	{
		public Tag Tag { get; set; }
		public string Url { get; set; }
		public string IncludeUrl { get; set; }
		public string ExcludeUrl { get; set; }
		public string Description { get; set; }
	}
}