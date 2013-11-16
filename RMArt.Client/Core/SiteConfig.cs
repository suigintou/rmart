namespace RMArt.Client.Core
{
	public class SiteConfig
	{
		public string AuthenticationUrl { get; set; }
		public string CheckByHashUrl { get; set; }
		public string UploadUrl { get; set; }
		public string EditUrl { get; set; }
		public string SearchUrl { get; set; }
		public string PictureUrl { get; set; }
		public string NotExistsCheckResponse { get; set; }
		public string SearchErrorPrefix { get; set; }
	}
}