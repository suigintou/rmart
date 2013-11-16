using System.Net;
using NUnit.Framework;

namespace RMArt.Core.UnitTests.Users
{
	[TestFixture]
	public class TicketsTests
	{
		[Test]
		public void Create()
		{
			var svc = new TicketsService(new TestTicketsRepository());
			var t = svc.Create(TicketType.PasswordReset, 123, IPAddress.None);
			Assert.AreEqual(t.Type, TicketType.PasswordReset);
			Assert.AreEqual(t.UserID, 123);
			Assert.AreEqual(t.RequestIP, IPAddress.None.GetAddressBytes());
			Assert.NotNull(t.Token);
		}

		[Test]
		public void Check()
		{
			var svc = new TicketsService(new TestTicketsRepository());
			var t = svc.Create(TicketType.PasswordReset, 123, IPAddress.None);
			Assert.AreEqual(svc.Check(TicketType.PasswordReset, t.Token), 123);
			Assert.Less(svc.Check(TicketType.EmailValidation, t.Token), 0);
			Assert.Less(svc.Check(TicketType.PasswordReset, "fake"), 0);
		}

		[Test]
		public void Expire()
		{
			var svc = new TicketsService(new TestTicketsRepository());
			var t = svc.Create(TicketType.PasswordReset, 123, IPAddress.None);
			svc.Expire(t.Token);
			Assert.Less(svc.Check(TicketType.PasswordReset, t.Token), 0);
		}
	}
}