using System.Linq;

namespace RMArt.Core
{
	public interface IUsersRepository
	{
		User GetByID(int id);
		User GetByLogin(string login);
		User GetByEmail(string email);
		UserRole? GetRole(int id);

		IQueryable<User> Find();
		int Add(User user);
		User Update(int id, UserUpdate update);
	}
}