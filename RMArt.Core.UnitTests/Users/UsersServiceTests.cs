using NUnit.Framework;

namespace RMArt.Core.UnitTests.Users
{
	[TestFixture]
	public class UsersServiceTests
	{
		[Test]
		public void CreateAndGet()
		{
			var repository = new TestUsersRepository();
			var service = new UsersService(repository);
			var id = service.CreateUser("test", "Test User", "test@test.com", true, "qwerty", UserRole.User);
			var created = service.GetByID(id);
			Assert.IsNotNull(created);
			Assert.AreEqual(created.Login, "test");
			Assert.AreEqual(created.Name, "Test User");
			Assert.AreEqual(created.Email, "test@test.com");
			Assert.AreEqual(created.Role, UserRole.User);
			Assert.IsTrue(service.CheckPassword(created, "qwerty"));
		}

		[Test]
		public void GetByLogin()
		{
			var repository = new TestUsersRepository();
			var service = new UsersService(repository);
			var id = service.CreateUser("test", "Test User", "test@test.com", true, "qwerty", UserRole.User);
			var user = service.GetByLogin("test");
			Assert.AreEqual(id, user.ID);
		}

		[Test]
		public void GetByEmail()
		{
			var repository = new TestUsersRepository();
			var service = new UsersService(repository);
			var id = service.CreateUser("test", "Test User", "test@test.com", true, "qwerty", UserRole.User);
			var user = service.GetByEmail("test@test.com");
			Assert.AreEqual(id, user.ID);
		}
	}
}