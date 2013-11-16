using System.ComponentModel;

namespace RMArt.ServiceTool
{
	public class RebuidThumbsOptions
	{
		[Description("Create new thumbnail and overwrite if exists.")]
		public bool OverwriteExisting { get; set; }
	}
}