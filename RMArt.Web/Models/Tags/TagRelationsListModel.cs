using System.Collections.Generic;
using RMArt.Core;

namespace RMArt.Web.Models
{
	public class TagRelationsListModel
	{
		public int TagID { get; set; }
		public IList<Tag> Relations { get; set; }
		public bool IsParents { get; set; }
		public bool ShowDeleteButtons { get; set; }
		public string ReturnUrl { get; set; }
	}
}