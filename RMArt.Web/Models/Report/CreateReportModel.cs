using System.ComponentModel.DataAnnotations;
using RMArt.Core;
using RMArt.Web.Resources;

namespace RMArt.Web.Models
{
	public class CreateReportModel
	{
		public ObjectType? TargetType { get; set; }
		
		public int? TargetID { get; set; }
		
		public ReportType? ReportType { get; set; }
		
		[Required(ErrorMessageResourceType = typeof(ReportsResources), ErrorMessageResourceName = "ErrorEmptyMessage")]
		[StringLength(2000, ErrorMessageResourceType = typeof(ReportsResources), ErrorMessageResourceName = "ErrorMessageTooBig")]
		public string Message { get; set; }
		
		public string Captcha { get; set; }
	}
}