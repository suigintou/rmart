using System.Net.Mail;

namespace RMArt.Core
{
	public static class MailHelper
	{
		public static void Send(string to, string subject, string body)
		{
			var client = new SmtpClient();
			client.SendCompleted += (s, e) => client.Dispose();
			var message = new MailMessage();
			message.To.Add(to);
			message.Subject = subject;
			message.Body = body;
			client.Send(message);
		}
	}
}