using System.Collections.Generic;

namespace RMArt.Web.Models
{
	public class TagAliasesListModel
	{
		public int TagID { get; set; }
		public ICollection<string> Aliases { get; set; }
		public bool ShowDeleteButtons { get; set; }
	}
}