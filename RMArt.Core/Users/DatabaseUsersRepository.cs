using System;
using System.Data;
using System.Linq;
using LinqToDB;

namespace RMArt.Core
{
	public class DatabaseUsersRepository : IUsersRepository
	{
		private readonly IDataContextProvider _dataContextProvider;

		public DatabaseUsersRepository(IDataContextProvider dataContextProvider)
		{
			if (dataContextProvider == null)
				throw new ArgumentNullException("dataContextProvider");

			_dataContextProvider = dataContextProvider;
		}

		public User GetByID(int id)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return context.Users().SingleOrDefault(u => u.ID == id);
		}

		public User GetByLogin(string login)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return context.Users().SingleOrDefault(u => u.Login == login);
		}

		public User GetByEmail(string email)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return context.Users().SingleOrDefault(u => u.Email == email);
		}

		public UserRole? GetRole(int id)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return context.Users().Where(u => u.ID == id).Select(u => (UserRole?)u.Role).SingleOrDefault();
		}

		public IQueryable<User> Find()
		{
			return _dataContextProvider.CreateDataContext().Users();
		}

		public int Add(User user)
		{
			using (var context = _dataContextProvider.CreateDataContext())
				return Convert.ToInt32(
					context
						.Users()
							.Value(_ => _.Login, user.Login)
							.Value(_ => _.PasswordHash, user.PasswordHash)
							.Value(_ => _.Name, user.Name)
							.Value(_ => _.Email, user.Email)
							.Value(_ => _.IsEmailConfirmed, user.IsEmailConfirmed)
							.Value(_ => _.Role, user.Role)
							.Value(_ => _.RegistrationDate, user.RegistrationDate)
							.Value(_ => _.RegistrationIP, user.RegistrationIP)
						.InsertWithIdentity());
		}

		public User Update(int id, UserUpdate update)
		{
			using (var context = _dataContextProvider.CreateDataContext())
			using (var transaction = new DataContextTransaction(context))
			{
				transaction.BeginTransaction(IsolationLevel.Serializable);

				var source = context.Users().Where(u => u.ID == id);
				var oldUser = source.SingleOrDefault();
				if (oldUser != null)
				{
					if (update.Login != null)
						source.Set(_ => _.Login, update.Login).Update();
					if (update.Name != null)
						source.Set(_ => _.Name, update.Name).Update();
					if (update.Email != null)
						source.Set(_ => _.Email, update.Email).Update();
					if (update.PasswordHash != null)
						source.Set(_ => _.PasswordHash, update.PasswordHash).Update();
					if (update.Role != null)
						source.Set(_ => _.Role, update.Role).Update();
					if (update.IsEmailChecked != null)
						source.Set(_ => _.IsEmailConfirmed, update.IsEmailChecked).Update();
				}

				transaction.CommitTransaction();

				return oldUser;
			}
		}
	}
}