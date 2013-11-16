using System.Collections.Generic;
using RMArt.Core;

namespace RMArt.Web.Models
{
	public class ManageUsersModel
	{
		public IList<User> Users { get; set; }
		public int TotalCount { get; set; }
		public int TotalPages { get; set; }
		public int CurrentPage { get; set; }
	}
}