using System.Collections.Generic;
using RMArt.Core;

namespace RMArt.Web.Models
{
	public class RatesListModel
	{
		public IList<Rate> Rates { get; set; }
		public int? PictureID { get; set; }
		public string User { get; set; }
		public int? Score { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public int TotalCount { get; set; }
		public bool ShowIPAddresses { get; set; }
	}
}