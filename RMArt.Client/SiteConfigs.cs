using System.Collections.Generic;

namespace RMArt.Client.Core
{
	public static class SiteConfigs
	{
		public static readonly Dictionary<string, SiteConfig> Configs =
			new Dictionary<string, SiteConfig>
			{
				{
					"RMArt.org",
					new SiteConfig
					{ 
						AuthenticationUrl="http://rmart.org/Account/LogIn",
						CheckByHashUrl="http://rmart.org/Upload/CheckExists?hash={hash}",
						SearchUrl = "http://rmart.org/Pictures/FindIDs?q={query}&sortBy={sortBy}",
						UploadUrl = "http://rmart.org/Upload/Batch",
						EditUrl = "http://rmart.org/Pictures/Edit",
						PictureUrl = "http://rmart.org/{id}/Src/{id}",
						NotExistsCheckResponse = "NOT_FOUND",
						SearchErrorPrefix = "Error:"
					}
				},
				{
					"localhost",
					new SiteConfig
					{ 
						AuthenticationUrl="http://localhost:8080/Account/LogIn",
						CheckByHashUrl="http://localhost:8080/Upload/CheckExists?hash={hash}",
						SearchUrl = "http://localhost:8080/Pictures/FindIDs?q={query}&sortBy={sortBy}",
						UploadUrl = "http://localhost:8080/Upload/Batch",
						EditUrl = "http://localhost:8080/Pictures/Edit",
						PictureUrl = "http://localhost:8080/{id}/Src/{id}",
						NotExistsCheckResponse = "NOT_FOUND",
						SearchErrorPrefix = "Error:"
					}
				}
			};
	}
}