using System.ComponentModel.DataAnnotations;
using RMArt.Core;

namespace RMArt.Web.Models
{
	public class CommentFormModel
	{
		[Required]
		public ObjectType ParentType { get; set; }

		public int ParentID { get; set; }

		[Required, StringLength(500)]
		public string Body { get; set; }
	}
}