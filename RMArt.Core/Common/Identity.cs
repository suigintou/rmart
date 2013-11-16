using System.Net;

namespace RMArt.Core
{
	public class Identity
	{
		public static readonly Identity Empty = new Identity(null, IPAddress.None);

		public int? UserID { get; private set; }
		public IPAddress IPAddress { get; private set; }

		public Identity(int? creatorID, IPAddress creatorIP)
		{
			UserID = creatorID;
			IPAddress = creatorIP;
		}
	}
}