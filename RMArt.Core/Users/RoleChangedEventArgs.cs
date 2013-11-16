using System;

namespace RMArt.Core
{
	public class RoleChangedEventArgs : EventArgs
	{
		public int UserID { get; private set; }
		public UserRole OldRole { get; private set; }
		public UserRole NewRole { get; private set; }

		public RoleChangedEventArgs(int userID, UserRole oldRole, UserRole newRole)
		{
			UserID = userID;
			OldRole = oldRole;
			NewRole = newRole;
		}
	}
}