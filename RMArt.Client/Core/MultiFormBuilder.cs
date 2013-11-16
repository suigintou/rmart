using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace RMArt.Client.Core
{
	internal class MultiFormBuilder : IDisposable
	{
		private readonly string _boundary;
		private readonly StreamWriter _writer;

		public MultiFormBuilder(WebRequest request, string boundary = null)
		{
			if (request == null)
				throw new ArgumentNullException("request");

			_boundary = boundary ?? DateTime.Now.ToBinary().ToString("X");

			request.ContentType = "multipart/form-data; boundary=" + _boundary;
			_writer = new StreamWriter(request.GetRequestStream());
		}

		public void AppendFormData(
			string name,
			byte[] content,
			string contentType,
			params KeyValuePair<string, string>[] fields)
		{
			if (name == null)
				throw new ArgumentNullException("name");
			if (content == null)
				throw new ArgumentNullException("content");
			if (contentType == null)
				throw new ArgumentNullException("contentType");

			AppendFormDataHeader(name, fields);
			_writer.WriteLine("Content-Type: " + contentType);
			_writer.WriteLine();
			_writer.Flush();
			_writer.BaseStream.Write(content, 0, content.Length);
			_writer.WriteLine();
		}

		public void AppendFormData(string name, string value)
		{
			if (name == null)
				throw new ArgumentNullException("name");

			AppendFormDataHeader(name);
			_writer.WriteLine();
			_writer.WriteLine(value);
		}

		#region Implementation of IDisposable

		public void Dispose()
		{
			_writer.WriteLine("--" + _boundary);
			_writer.Dispose();
		}

		#endregion

		private void AppendFormDataHeader(
			string name,
			params KeyValuePair<string, string>[] fields)
		{
			_writer.WriteLine("--" + _boundary);
			_writer.Write("Content-Disposition: form-data; name=\"{0}\"", name);
			foreach (var field in fields)
				_writer.Write("; {0}=\"{1}\"", field.Key, field.Value);
			_writer.WriteLine();
		}
	}
}