using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using HttpServer;
using HttpServer.HttpModules;
using HttpServer.Sessions;

namespace Possan.Distributed.Manager
{
	class ManagerRequestHandler : HttpModule
	{
		private readonly Manager _manager;

		public ManagerRequestHandler(Manager manager)
		{
			_manager = manager;
		}


		public override bool Process(IHttpRequest request, IHttpResponse response, IHttpSession session)
		{
			if (request.UriPath == "/keepalive")
			{
				// var r = new BinaryReader(request.Body);
				var reader = new StreamReader(request.Body);
				string posttext = reader.ReadToEnd();
				_manager.Controller.Keepalive(posttext);
				response.Body = new MemoryStream(response.Encoding.GetBytes("OK"));
				return true;
			}

			if (request.UriPath == "/createjob")
			{
				var reader = new StreamReader(request.Body);
				string posttext = reader.ReadToEnd();
				var jobid = _manager.Controller.CreateJob(posttext);
				if (string.IsNullOrEmpty(jobid))
					return false;
				response.Encoding = Encoding.UTF8;
				response.Body = new MemoryStream(response.Encoding.GetBytes(jobid));
				return true;
			}

			var m2 = Regex.Match(request.UriPath, "/job/([0-9a-z-]+)/assemblies");
			if (m2.Success)
			{
				var jobid = m2.Groups[1].Value;
				var bytes  = Utilities.ReadAllBytes(request.Body);
				_manager.Controller.AddAssembly(jobid, bytes);
				response.Body = new MemoryStream(response.Encoding.GetBytes("OK"));
				return true;
			}

			m2 = Regex.Match(request.UriPath, "/job/([0-9a-z-]+)/worker-callback");
			if (m2.Success)
			{
				var jobid = m2.Groups[1].Value;
				var reader = new StreamReader(request.Body);
				string posttext = reader.ReadToEnd();
				_manager.Controller.JobDone(jobid, posttext);
				// Console.WriteLine("Worker: Start job #" + m2.Groups[1].Value);
				response.Body = new MemoryStream(response.Encoding.GetBytes("OK"));
				return true;
			}

			m2 = Regex.Match(request.UriPath, "/job/([0-9a-z-]+)/start");
			if (m2.Success)
			{
				var jobid = m2.Groups[1].Value;
				// var reader = new StreamReader(request.Body);
				// string posttext = reader.ReadToEnd();
				_manager.Controller.Start(jobid);
				// Console.WriteLine("Worker: Start job #" + m2.Groups[1].Value);
				response.Body = new MemoryStream(response.Encoding.GetBytes("OK"));
				return true;
			}

			m2 = Regex.Match(request.UriPath, "/job/([0-9a-z-]+)/status");
			if (m2.Success)
			{
				var jobid = m2.Groups[1].Value;
				var status = _manager.Controller.GetStatus(jobid);
				response.Encoding = Encoding.UTF8;
				response.Body = new MemoryStream(response.Encoding.GetBytes(status));
				// var reader = new StreamReader(request.Body);
				// string posttext = reader.ReadToEnd();
				// Console.WriteLine("Worker: Start job #" + m2.Groups[1].Value);)
				return true;
			}


			Console.WriteLine("Unhandled manager request: " + request.UriPath);

			/*
		response.Encoding = Encoding.UTF8;
		string body = "Hej från manager; localpath=" + request.UriPath + ", querystring=" + request.QueryString + ", method=" + request.Method;
		response.Body = new MemoryStream(response.Encoding.GetBytes(body));
			*/

			return false;
		}
	}
}
