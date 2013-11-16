using System;
using System.Net;
using RMArt.Core;

namespace RMArt.Web.Models
{
	public class DiscussionMessageModel
	{
		public int ID { get; set; }
		public ObjectReference Parent { get; set; }
		public string Body { get; set; }
		public int CreatorID { get; set; }
		public string CreatorLogin { get; set; }
		public string CreatorEmeil { get; set; }
		public bool CanViewPrivateData { get; set; }
		public IPAddress CreatedFrom { get; set; }
		public DateTime CreatedAt { get; set; }
		public ModerationStatus Status { get; set; }
	}
}