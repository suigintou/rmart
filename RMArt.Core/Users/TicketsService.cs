using System;
using System.Net;
using System.Security.Cryptography;

namespace RMArt.Core
{
	public class TicketsService : ITicketsService, IDisposable
	{
		private readonly ITicketsRepository _ticketsRepository;
		private readonly RandomNumberGenerator _random = new RNGCryptoServiceProvider();

		public TicketsService(ITicketsRepository ticketsRepository)
		{
			_ticketsRepository = ticketsRepository;
		}

		public void Dispose()
		{
			_random.Dispose();
		}

		public Ticket Create(TicketType type, int userID, IPAddress requestIP)
		{
			var creationDate = DateTime.UtcNow;
			var ticket =
				new Ticket
				{
					Type = type,
					UserID = userID,
					CreationDate = creationDate,
					Token = GenerateToken(),
					RequestIP = requestIP.GetAddressBytes()
				};
			_ticketsRepository.Add(ticket);
			return ticket;
		}

		public void Expire(string token)
		{
			_ticketsRepository.Remove(token);
		}

		public void Expire(TicketType type, int userID)
		{
			_ticketsRepository.Remove(type, userID);
		}

		public int Check(TicketType type, string token)
		{
			var ticket = _ticketsRepository.Get(token);
			if (ticket == null)
				return -1;
			if (ticket.CreationDate + GetLifemateTime(type) <= DateTime.UtcNow)
			{
				Expire(ticket.Token);
				return -1;
			}
			if (ticket.Type != type)
				return -1;
			return ticket.UserID;
		}

		private string GenerateToken()
		{
			while (true)
			{
				var tokenData = new byte[16];
				_random.GetBytes(tokenData);
				var token = tokenData.ToHexString();
				if (!_ticketsRepository.IsExists(token))
					return token;
			}
		}

		private static TimeSpan GetLifemateTime(TicketType type)
		{
			switch (type)
			{
				case TicketType.EmailValidation: return TimeSpan.FromDays(3);
				case TicketType.PasswordReset: return TimeSpan.FromDays(1);
				default: throw new NotSupportedException();
			}
		}
	}
}