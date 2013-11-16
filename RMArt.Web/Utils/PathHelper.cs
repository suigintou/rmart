using System;
using System.Web;

namespace RMArt.Web
{
	public static class PathHelper
	{
		public static string ToPhysicalPath(this HttpServerUtility server, string path)
		{
			if (path == null)
				return null;

			if (path.StartsWith("~", StringComparison.Ordinal) || path.StartsWith("/", StringComparison.Ordinal))
				return server.MapPath(path);
			
			return path;
		}
	}
}